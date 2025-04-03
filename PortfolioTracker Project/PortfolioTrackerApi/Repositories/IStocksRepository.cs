using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Repositories
{
    public interface IStocksRepository
    {
        Task<List<StockPrice>> GetAllStocksAsync();
        Task<StockPrice> GetStockBySymbolAsync(string ticker);
        Task AddStockAsync(StockPrice stock);
        Task SaveChangesAsync();

        Task AddPortfolioStockAsync(Stock stock);
    }
}
