using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.DAL;
using PortfolioTrackerApi.DTOS;
using System.Text.Json;

namespace PortfolioTrackerApi.Services
{
    public class StockPriceUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<StockPriceUpdaterService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private int _apiCallCount = 0; // Track daily API calls
        private DateTime _lastApiResetDate = DateTime.UtcNow.Date; // Track last reset date

        public StockPriceUpdaterService(IServiceScopeFactory serviceScopeFactory, ILogger<StockPriceUpdaterService> logger, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Reset API call count at midnight UTC
                if (DateTime.UtcNow.Date > _lastApiResetDate)
                {
                    _apiCallCount = 0;
                    _lastApiResetDate = DateTime.UtcNow.Date;
                    _logger.LogInformation("API call limit reset for the new day.");
                }

                if (_apiCallCount < 25) // Only proceed if API limit is not reached
                {
                    await UpdateStockPrices();
                }
                else
                {
                    _logger.LogWarning("API call limit reached. Skipping updates until midnight.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait 1 minute before next update
            }
        }

        private async Task UpdateStockPrices()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var stocksToUpdate = await dbContext.StocksPrice
                    .OrderBy(s => s.LastUpdated)
                    .Take(5)
                    .ToListAsync();

                if (!stocksToUpdate.Any())
                {
                    _logger.LogInformation("No stocks found to update.");
                    return;
                }

                foreach (var stock in stocksToUpdate)
                {
                    if (_apiCallCount >= 25)
                    {
                        _logger.LogWarning("API call limit reached. Stopping further updates.");
                        return; // Stop further API calls
                    }

                    try
                    {
                        string ApiKey = _configuration["AlphaVantage:ApiKey"];
                        string requestUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={stock.Ticker}.BSE&apikey={ApiKey}";
                        HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            var stockData = JsonSerializer.Deserialize<GlobalQuoteResponse>(jsonResponse);

                            if (stockData?.GlobalQuote != null)
                            {
                                stock.CurrentPrice = decimal.Parse(stockData.GlobalQuote.Price);
                                stock.LastUpdated = DateTime.UtcNow;
                                _apiCallCount++; // Increase API call count

                                _logger.LogInformation($"Updated {stock.Ticker}: ₹{stock.CurrentPrice}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error updating stock price for {stock.Ticker}: {ex.Message}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
