using StockPriceWorkerService.DB;
using StockPriceWorkerService.Models;
using StockPriceWorkerService.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceWorkerService.Services
{
    class StockPriceGeneratorService:BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRedisService _redisService;
        private readonly Random _random = new();
        private DateTime _lastPersistedHour = DateTime.UtcNow;

        public StockPriceGeneratorService(IServiceScopeFactory scopeFactory, IRedisService redisService)
        {
            _scopeFactory = scopeFactory;
            _redisService = redisService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await GenerateAndStoreStockPricesAsync();

                if ((DateTime.UtcNow - _lastPersistedHour).TotalMinutes >= 30)
                {
                    await PersistHistoricalPricesAsync();
                    _lastPersistedHour = DateTime.UtcNow;
                }

                await _redisService.PublishAsync("stock-updates", "prices-updated");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task GenerateAndStoreStockPricesAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IStockPriceRepository>();

            var stocks = await repository.GetAllAsync();
            var updatedStocks = new ConcurrentBag<StockPrice>();

            Parallel.ForEach(stocks, stock =>
            {
                decimal minPrice = stock.CurrentPrice * 0.95m;
                decimal maxPrice = stock.CurrentPrice * 1.05m;

                var newPrice = Math.Round((decimal)(_random.NextDouble() * (double)(maxPrice - minPrice) + (double)minPrice), 2);

                stock.CurrentPrice = newPrice;
                stock.LastUpdated = DateTime.UtcNow;
                updatedStocks.Add(stock);
            });

            await _redisService.SetStockPricesAsync(updatedStocks.ToList());
        }

        private async Task PersistHistoricalPricesAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IStockPriceRepository>();
            var redisService = _redisService;
            var historicalRepo = scope.ServiceProvider.GetRequiredService<IHistoricalStockPriceRepository>();

            var cachedPrices = await redisService.GetStockPricesAsync();

            var historicalPrices = cachedPrices.Select(stock =>
            {
                // Simulate opening price around ±2% of current price
                decimal openingPrice = Math.Round(stock.CurrentPrice * GetRandomFactor(0.98m, 1.02m), 2);

                // Simulate closing price around ±2% of current price
                decimal closingPrice = Math.Round(stock.CurrentPrice * GetRandomFactor(0.98m, 1.02m), 2);

                // Get min and max for high/low price based on opening and closing
                decimal low = Math.Min(openingPrice, closingPrice) * GetRandomFactor(0.98m, 1.00m);
                decimal high = Math.Max(openingPrice, closingPrice) * GetRandomFactor(1.00m, 1.02m);

                return new HistoricalStockPrice
                {
                    Ticker = stock.Ticker,
                    Date = DateTime.UtcNow,
                    OpeningPrice = openingPrice,
                    ClosingPrice = closingPrice,
                    HighPrice = Math.Round(high, 2),
                    LowPrice = Math.Round(low, 2)
                };
            }).ToList();

           
            await historicalRepo.AddRangeAsync(historicalPrices);
            await historicalRepo.SaveChangesAsync();

            var existingStockPrices = await repository.GetAllAsync();

            foreach (var stock in existingStockPrices)
            {
                var redisStock = cachedPrices.FirstOrDefault(s => s.Ticker == stock.Ticker);
                if (redisStock != null)
                {
                    stock.CurrentPrice = redisStock.CurrentPrice;
                    stock.LastUpdated = DateTime.UtcNow;
                }
            }

            await repository.UpdateStockPricesAsync(existingStockPrices);
        }
        // Helper method
        decimal GetRandomFactor(decimal min, decimal max)
        {
            return (decimal)(_random.NextDouble() * (double)(max - min) + (double)min);
        }
    }
}
