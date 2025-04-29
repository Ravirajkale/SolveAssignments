using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Service_Interfaces;
using System.Text.Json;

namespace PortfolioTrackerApi.Services.StockPriceApi
{
    public class TwelveDataService : IStockPriceApiService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public TwelveDataService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<StockPrice> GetStockPriceAsync(string ticker)
        {
            string apiKey = _configuration["TwelveData:ApiKey"];
            string url = $"https://api.twelvedata.com/price?symbol={ticker}&apikey={apiKey}";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var json = JsonDocument.Parse(response);

                if (json.RootElement.TryGetProperty("status", out var status) &&
                    status.GetString() == "error")
                {
                    Console.WriteLine($"TwelveData API error for {ticker}: {json.RootElement.GetProperty("message").GetString()}");
                    return null;
                }

                if (!json.RootElement.TryGetProperty("price", out var priceProp))
                {
                    Console.WriteLine($"Price property not found for ticker {ticker}");
                    return null;
                }

                return new StockPrice
                {
                    Ticker = ticker,
                    Company = ticker, // You could optionally call TwelveData's `symbol_search` for the full company name
                    CurrentPrice = decimal.Parse(priceProp.GetString()),
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in TwelveDataService for ticker {ticker}: {ex.Message}");
                return null;
            }
        }
    }
}
