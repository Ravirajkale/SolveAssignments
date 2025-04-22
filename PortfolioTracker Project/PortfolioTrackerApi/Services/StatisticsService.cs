using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Repositories;

namespace PortfolioTrackerApi.Services
{
    public class StatisticsService:IStatisticsService
    {
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IStocksRepository _stocksRepository;
        private readonly IRedisService _redisService;
        private readonly IHistoricalStockPriceRepository _historicalRepo;

        public StatisticsService(
            IPortfolioRepository portfolioRepo,
            IRedisService redisService,
            IHistoricalStockPriceRepository historicalRepo,
            IStocksRepository stocksRepository)
        {
            _portfolioRepo = portfolioRepo;
            _redisService = redisService;
            _historicalRepo = historicalRepo;
            _stocksRepository = stocksRepository;
        }

        public async Task<PortfolioSummaryDto> GetPortfolioSummaryAsync(string userId)
        {
            var portfolios = await _portfolioRepo.GetUserPortfoliosWithStocksAsync(userId);
            var currentPrices = await _redisService.GetStockPricesAsync();

            decimal totalInvested = 0;
            decimal totalCurrentValue = 0;
            int portfoliosInProfit = 0;
            int portfoliosInLoss = 0;
            int portfoliosWithNoStocks = 0;

            foreach (var portfolio in portfolios)
            {
                if (portfolio.Stocks == null || !portfolio.Stocks.Any())
                {
                    portfoliosWithNoStocks++;
                    continue;
                }

                decimal portfolioInvested = 0;
                decimal portfolioCurrentValue = 0;

                foreach (var stock in portfolio.Stocks)
                {
                    var currentPrice = currentPrices.FirstOrDefault(p => p.Ticker == stock.Ticker)?.CurrentPrice ?? 0;

                    portfolioInvested += stock.Quantity * stock.PurchasePrice;
                    portfolioCurrentValue += stock.Quantity * currentPrice;
                }

                totalInvested += portfolioInvested;
                totalCurrentValue += portfolioCurrentValue;

                if (portfolioCurrentValue > portfolioInvested)
                    portfoliosInProfit++;
                else if (portfolioCurrentValue < portfolioInvested)
                    portfoliosInLoss++;
            }

            return new PortfolioSummaryDto
            {
                TotalInvested = Math.Round(totalInvested, 2),
                TotalCurrentValue = Math.Round(totalCurrentValue, 2),
                OverallProfitLoss = Math.Round(totalCurrentValue - totalInvested, 2),
                PortfoliosInProfit = portfoliosInProfit,
                PortfoliosInLoss = portfoliosInLoss,
                PortfoliosWithNoStocks = portfoliosWithNoStocks
            };
        }

        public async Task<List<HistoricalChartPointDto>> GetHistoricalChartDataAsync(string ticker, DateTime? date = null)
        {
            var prices = await _historicalRepo.GetByTickerAsync(ticker);

            if (date.HasValue)
            {
                var selectedDate = date.Value.Date;
                prices = prices
                    .Where(p => p.Date.Date == selectedDate)
                    .OrderBy(p => p.Date)
                    .ToList();
            }

            return prices.Select(p => new HistoricalChartPointDto
            {
                Date = p.Date,
                ClosingPrice = p.ClosingPrice
            }).ToList();
        }

        public async Task<Dictionary<string, List<HistoricalChartPointDto>>> GetDefaultHistoricalChartDataAsync(string date)
        {
            var parsedDate = DateOnly.Parse(date);
            var tickers = await _historicalRepo.GetRandomTickersByDateAsync(parsedDate, 5);
            var result = new Dictionary<string, List<HistoricalChartPointDto>>();

            foreach (var ticker in tickers)
            {
                var prices = await _historicalRepo.GetByTickerAndDateAsync(ticker, parsedDate);

                if (prices.Any())
                {
                    result[ticker] = prices.Select(p => new HistoricalChartPointDto
                    {
                        Date = p.Date, // include full datetime for chart
                        ClosingPrice = p.ClosingPrice
                    }).ToList();
                }
            }

            return result;
        }

        public async Task<List<string>> GetAvailableChartDatesAsync()
        {
            var allPrices = await _historicalRepo.GetAllAsync(); // Create this if not already implemented
            var uniqueDates = allPrices
                .Select(p => p.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .Select(d => d.ToString("yyyy-MM-dd"))
                .ToList();

            return uniqueDates;
        }
    }
}
