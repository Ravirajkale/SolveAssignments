using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Repositories;
using System.Text.Json;

namespace PortfolioTrackerApi.Services
{

    public class StocksService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IStocksRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IRedisService _redisService;

        public StocksService(HttpClient httpClient, IConfiguration configuration, IStocksRepository stockRepository, IPortfolioRepository portfolioRepository, IRedisService redisService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _stockRepository = stockRepository;
            _portfolioRepository = portfolioRepository;
            _redisService = redisService;
        }

        public async Task<List<StockPrice>> GetAvailableStocksAsync()
        {
            var cachedStocks = await _redisService.GetStockPricesAsync();
            if (cachedStocks.Any()) return cachedStocks.Take(20).ToList();

            var stocks = await _stockRepository.GetAllStocksAsync();
            await _redisService.SetStockPricesAsync(stocks);
            return stocks;
        }

        public async Task<StockPrice> GetStockWithCacheAsync(string ticker)
        {
            var cachedStocks = await _redisService.GetStockPricesAsync();
            var stock = cachedStocks.FirstOrDefault(s => s.Ticker == ticker);
            if (stock != null) return stock;

            string lockKey = $"lock:stock:{ticker}";
            var lockAcquired = await _redisService.GetValueAsync(lockKey);
            if (!string.IsNullOrEmpty(lockAcquired))
            {
                // Wait and retry fetching from cache
                await Task.Delay(500);
                cachedStocks = await _redisService.GetStockPricesAsync();
                stock = cachedStocks.FirstOrDefault(s => s.Ticker == ticker);
                if (stock != null) return stock;
            }

            await _redisService.SetValueAsync(lockKey, "locked", TimeSpan.FromSeconds(10));
            stock = await _stockRepository.GetStockBySymbolAsync(ticker);
            if (stock == null)
            {
                stock = await FetchStockFromAPI(ticker);
                if (stock == null) return null;
            }

            cachedStocks.Add(stock);
            await _redisService.SetStockPricesAsync(cachedStocks);
            await _redisService.SetValueAsync(lockKey, "", TimeSpan.FromSeconds(1)); // Release lock
            return stock;
        }

        public async Task<List<StockDto>> GetPortfolioStocksAsync(int portfolioId)
        {
            var portfolio = await _portfolioRepository.GetPortfolioStocksAsync(portfolioId);
            if (portfolio == null) throw new Exception("Portfolio not found");

            var cachedStocks = await _redisService.GetStockPricesAsync();
            var portfolioStocks = new List<StockDto>();

            foreach (var stock in portfolio.Stocks)
            {
                var stockPrice = cachedStocks.FirstOrDefault(s => s.Ticker == stock.Ticker) ?? await _stockRepository.GetStockBySymbolAsync(stock.Ticker);
                portfolioStocks.Add(new StockDto
                {
                    StockId = stock.Id,
                    Ticker = stock.Ticker,
                    Company = stock.Name,
                    CurrentPrice = stockPrice?.CurrentPrice ?? 0,
                    PurchasePrice = stock.PurchasePrice,
                    Quantity = stock.Quantity,
                    TotalCurrentValue = (stockPrice?.CurrentPrice ?? 0) * stock.Quantity,
                    TotalPurchasedValue = stock.PurchasePrice * stock.Quantity
                });
            }

            return portfolioStocks;
        }

        public async Task<StockPrice> FetchStockFromAPI(string ticker)
        {
            string apiKey = _configuration["AlphaVantage:ApiKey"];
            string url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}.BSE&apikey={apiKey}";

            var response = await _httpClient.GetStringAsync(url);
            Console.WriteLine(response);
            var json = JsonDocument.Parse(response);
            var root = json.RootElement;

            if (!root.TryGetProperty("Global Quote", out var stockData) || stockData.GetRawText() == "{}")
            {
                Console.WriteLine($"Stock data for ticker {ticker} not found.");
                return null;
            }

            if (!stockData.TryGetProperty("05. price", out var priceProperty) || string.IsNullOrWhiteSpace(priceProperty.GetString()))
            {
                Console.WriteLine($"Price data for ticker {ticker} not available.");
                return null;
            }

            var stock = new StockPrice
            {
                Ticker = ticker,
                Company = ticker,
                CurrentPrice = decimal.Parse(stockData.GetProperty("05. price").GetString()),
                LastUpdated = DateTime.UtcNow
            };

            await _stockRepository.AddStockAsync(stock);
            await _stockRepository.SaveChangesAsync();

            var cachedStocks = await _redisService.GetStockPricesAsync();
            cachedStocks.Add(stock);
            await _redisService.SetStockPricesAsync(cachedStocks);

            return stock;
        }

        internal async Task AddStockToPortfolioAsync(StockAddDto stockDto)
        {
            var portfolio = await _portfolioRepository.GetPortfolioByIdAsync(stockDto.PortfolioId);
            if (portfolio == null) throw new Exception("Portfolio not found");

            var stock = new Stock
            {
                PortfolioId = stockDto.PortfolioId,
                Name = stockDto.Name,
                Ticker = stockDto.Ticker,
                Quantity = stockDto.Quantity,
                PurchasePrice = stockDto.PurchasePrice,
                PurchasedAt = DateTime.UtcNow
            };

            await _stockRepository.AddPortfolioStockAsync(stock);
        }

    }
}