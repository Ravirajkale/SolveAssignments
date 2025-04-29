using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Repositories;
using PortfolioTrackerApi.Service_Interfaces;

namespace PortfolioTrackerApi.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IRedisService redisService;
        private readonly IStocksRepository stocksRepository;
        public PortfolioService(IPortfolioRepository portfolioRepository, IRedisService redisService, IStocksRepository stocksRepository)
        {
            this.portfolioRepository = portfolioRepository;
            this.redisService = redisService;
            this.stocksRepository = stocksRepository;
        }

        public async Task<PortfolioResponseDto> CreatePortfolioAsync(string userId, PortfolioRequestDtocs portfolioRequest)
        {
            var entity = new Portfolio()
            {
                Name=portfolioRequest.Name,
                UserId = userId

            };

            await portfolioRepository.AddPortfolioAsync(entity);

            return new PortfolioResponseDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public async Task<(IEnumerable<PortfolioResponseDto>, int)> GetPaginatedPortfoliosAsync(string userId, int pageNumber, int pageSize)
        {
            var portfoliosWithStocks = await portfolioRepository.GetUserPortfoliosWithStocksAsync(userId);
            var allCachedPrices = await redisService.GetStockPricesAsync();
            if (allCachedPrices == null || allCachedPrices.Count==0)
                allCachedPrices = await stocksRepository.GetAllStocksAsync();
            var portfolioDtos = portfoliosWithStocks
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(portfolio =>
                {
                    var purchaseValue = portfolio.Stocks.Sum(s => s.Quantity * s.PurchasePrice);
                    var currentValue = portfolio.Stocks.Sum(s =>
                    {
                        var cached = allCachedPrices.FirstOrDefault(cp => cp.Ticker == s.Ticker);
                        return cached != null ? s.Quantity * cached.CurrentPrice : 0;
                    });

                    return new PortfolioResponseDto
                    {
                        Id = portfolio.Id,
                        Name = portfolio.Name,
                        StocksCount = portfolio.Stocks.Count,
                        PurchaseValue = purchaseValue,
                        CurrentValue = currentValue
                    };
                }).ToList();

            return (portfolioDtos, portfoliosWithStocks.Count());
         //   return (portfolios, totalCount);

        }
        public async Task<bool> UpdateQuantityAsync(int stockId, int newQuantity)
        {
            var stock = await stocksRepository.GetByIdAsync(stockId);
            if (stock == null) return false;

            stock.Quantity = newQuantity;
            await stocksRepository.UpdateAsync(stock);
            await stocksRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStockAsync(int stockId)
        {
            var stock = await stocksRepository.GetByIdAsync(stockId);
            if (stock == null) return false;

            await stocksRepository.DeleteAsync(stock);
            await stocksRepository.SaveChangesAsync();
            return true;
        }
    }
}
