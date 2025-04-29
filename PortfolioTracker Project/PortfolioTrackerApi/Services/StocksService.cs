using PortfolioTrackerApi.Controllers;
using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Repositories;
using PortfolioTrackerApi.Service_Interfaces;
using PortfolioTrackerApi.Services.StockPriceApi;
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
        private readonly StockPriceApiManager _apiManager;

        public StocksService(HttpClient httpClient, IConfiguration configuration, IStocksRepository stockRepository, IPortfolioRepository portfolioRepository, IRedisService redisService,StockPriceApiManager apiManager)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _stockRepository = stockRepository;
            _portfolioRepository = portfolioRepository;
            _redisService = redisService;
            _apiManager = apiManager;
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
                await Task.Delay(500);
                cachedStocks = await _redisService.GetStockPricesAsync();
                stock = cachedStocks.FirstOrDefault(s => s.Ticker == ticker);
                if (stock != null) return stock;
            }

            await _redisService.SetValueAsync(lockKey, "locked", TimeSpan.FromSeconds(10));

            stock = await _stockRepository.GetStockBySymbolAsync(ticker);
            if (stock == null)
            {
                stock = await _apiManager.GetStockPriceWithFallbackAsync(ticker);
                if (stock == null) return null;

                await _stockRepository.AddStockAsync(stock);
                await _stockRepository.SaveChangesAsync();
            }

            cachedStocks.Add(stock);
            await _redisService.SetStockPricesAsync(cachedStocks);
            await _redisService.SetValueAsync(lockKey, "", TimeSpan.FromSeconds(1));

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

        internal async Task SaveStockToPortfolioAsync(List<ScrapperDto> stockDtos)
        {
            List<StockPrice> stocks = new List<StockPrice>();
            foreach (var stockDto in stockDtos) {
                var stock = await _stockRepository.GetStockBySymbolAsync(stockDto.ticker);
                if (stock == null)
                {
                    var newStock = new StockPrice
                    {
                        CurrentPrice = Convert.ToDecimal(stockDto.last_price.Replace(",", "")),
                        
                        Company = stockDto.ticker,
                        Ticker = stockDto.ticker,
                        LastUpdated = DateTime.UtcNow
                    };
                    if (stockDto.currency == "USD")
                        newStock.CurrentPrice = Math.Round(newStock.CurrentPrice / 0.012m, 2);
                    await _stockRepository.AddStockAsync(newStock);
                    stocks.Add(newStock);
                }
                else
                {
                    stock.CurrentPrice = Convert.ToDecimal(stockDto.last_price.Replace(",", ""));
                
                     
                    stock.LastUpdated = DateTime.UtcNow;
                    await _stockRepository.UpdateAsync(stock);
                    stocks.Add(stock);
                }
                await _stockRepository.SaveChangesAsync();

            }
           await _redisService.SetStockPricesAsync(stocks);
        }
    }
}