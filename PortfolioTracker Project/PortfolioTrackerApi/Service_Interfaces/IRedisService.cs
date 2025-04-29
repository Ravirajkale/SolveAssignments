using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;
using StackExchange.Redis;

namespace PortfolioTrackerApi.Service_Interfaces
{
    public interface IRedisService
    {
        Task<StockPriceDto?> GetPriceAsync(string symbol);
        Task<List<StockPrice>> GetStockPricesAsync();
        Task<string> GetValueAsync(string key);
        Task SetStockPricesAsync(List<StockPrice> stocks);

        Task SetValueAsync(string key, string value, TimeSpan expiry);
        ISubscriber GetSubscriber();
        Task<List<HistoricalStockPrice>> GetHistoricalPricesAsync(string ticker);
        Task SetHistoricalPricesAsync(string ticker, List<HistoricalStockPrice> prices);
        Task<List<HistoricalStockPrice>> GetHistoricalPricesByDateAsync(string ticker, DateOnly date);
        Task SetHistoricalPricesByDateAsync(string ticker, DateOnly date, List<HistoricalStockPrice> prices);
    }
}
