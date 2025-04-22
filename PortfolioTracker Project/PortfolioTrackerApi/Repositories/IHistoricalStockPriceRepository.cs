using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Repositories
{
    public interface IHistoricalStockPriceRepository
    {
        Task<List<HistoricalStockPrice>> GetByTickerAsync(string ticker);
        Task AddRangeAsync(List<HistoricalStockPrice> prices);
        Task SaveChangesAsync();
        Task<List<HistoricalStockPrice>> GetAllAsync();
        Task<List<HistoricalStockPrice>> GetByTickerAndDateAsync(string ticker, DateOnly date);
        Task<List<string>> GetRandomTickersByDateAsync(DateOnly date, int count);
    }
}
