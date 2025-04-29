using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Service_Interfaces
{
    public interface IStockPriceApiService
    {
        Task<StockPrice> GetStockPriceAsync(string ticker);
    }
}
