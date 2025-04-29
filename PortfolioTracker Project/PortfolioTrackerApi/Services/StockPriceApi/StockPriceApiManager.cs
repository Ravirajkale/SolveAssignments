using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Service_Interfaces;

namespace PortfolioTrackerApi.Services.StockPriceApi
{
    public class StockPriceApiManager
    {
        private readonly IEnumerable<IStockPriceApiService> _apiServices;

        public StockPriceApiManager(IEnumerable<IStockPriceApiService> apiServices)
        {
            _apiServices = apiServices;
        }

        public async Task<StockPrice> GetStockPriceWithFallbackAsync(string ticker)
        {
            foreach (var service in _apiServices)
            {
                var stock = await service.GetStockPriceAsync(ticker);
                if (stock != null)
                {
                    if (service is TwelveDataService)
                    {
                        stock.CurrentPrice = Math.Round(stock.CurrentPrice / 0.012m, 2); // Convert USD to INR
                    }
                    return stock;
                }
                   
            }

            return null;
        }
    }
}
