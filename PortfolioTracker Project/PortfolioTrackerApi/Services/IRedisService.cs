using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Services
{
    public interface IRedisService
    {
        Task<List<StockPrice>> GetStockPricesAsync();
        Task<string> GetValueAsync(string key);
        Task SetStockPricesAsync(List<StockPrice> stocks);

        Task SetValueAsync(string key, string value, TimeSpan expiry);
    }
}
