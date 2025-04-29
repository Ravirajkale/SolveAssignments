using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Repositories
{
    public interface IStocksRepository
    {
        Task<List<StockPrice>> GetAllStocksAsync();
        Task<StockPrice> GetStockBySymbolAsync(string ticker);
        Task AddStockAsync(StockPrice stock);
        Task SaveChangesAsync();
        Task<List<Stock>> GetPortfolioStocks(int portfolioid);
        Task AddPortfolioStockAsync(Stock stock);
        Task<List<StockPrice>> GetStocksStartingWith(string query);
        Task<List<string>> GetRandomStockSymbolsAsync(int count);
        Task DeleteAsync(Stock stock);
        Task<Stock> GetByIdAsync(int stockId);
        Task UpdateAsync(Stock stock);
        Task UpdateAsync(StockPrice stock);
    }
}
