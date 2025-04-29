using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Service_Interfaces;
using System.Text.Json;

namespace PortfolioTrackerApi.Services.StockPriceApi
{
    public class AlphaVantageService : IStockPriceApiService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AlphaVantageService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<StockPrice> GetStockPriceAsync(string ticker)
        {
            string apiKey = _configuration["AlphaVantage:ApiKey"];
            string url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}.BSE&apikey={apiKey}";

            var response = await _httpClient.GetStringAsync(url);
            Console.WriteLine(response);
            var json = JsonDocument.Parse(response);
            var root = json.RootElement;

            if (!root.TryGetProperty("Global Quote", out var stockData) || stockData.GetRawText() == "{}")
            {
                Console.WriteLine($"Stock data for ticker {ticker} not found.");
                return null;
            }

            if (!stockData.TryGetProperty("05. price", out var priceProperty) || string.IsNullOrWhiteSpace(priceProperty.GetString()))
            {
                Console.WriteLine($"Price data for ticker {ticker} not available.");
                return null;
            }

            var stock = new StockPrice
            {
                Ticker = ticker,
                Company = ticker,
                CurrentPrice = decimal.Parse(stockData.GetProperty("05. price").GetString()),
                LastUpdated = DateTime.UtcNow
            };

        

            return stock;
        }
    }
}
