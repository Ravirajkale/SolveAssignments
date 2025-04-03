using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.DAL;
using PortfolioTrackerApi.Entities;
using System.Collections.Concurrent;

namespace PortfolioTrackerApi.Services
{
    public class StockPriceGeneratorService:BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRedisService _redisService;
        private readonly WebSocketHandler _webSocketHandler;
        private readonly Random _random = new();

        public StockPriceGeneratorService(IServiceScopeFactory scopeFactory, IRedisService redisService, WebSocketHandler webSocketHandler)
        {
            _scopeFactory = scopeFactory;
            _redisService = redisService;
            _webSocketHandler = webSocketHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await GenerateRandomStockPrices();
                await _webSocketHandler.NotifyStockPriceUpdate(); // Notify WebSocket clients
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task GenerateRandomStockPrices()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var stocks = await dbContext.StocksPrice.ToListAsync();
            var updatedStocks = new ConcurrentBag<StockPrice>(); // Thread-safe collection
            var random = new Random();

            Parallel.ForEach(stocks, stock =>
            {
                decimal minPrice = stock.CurrentPrice * 0.95m; // -5%
                decimal maxPrice = stock.CurrentPrice * 1.05m; // +5%
                stock.CurrentPrice = Math.Round((decimal)(random.NextDouble() * (double)(maxPrice - minPrice) + (double)minPrice), 2);
                stock.LastUpdated = DateTime.UtcNow;

                updatedStocks.Add(stock);
            });

            await _redisService.SetStockPricesAsync(updatedStocks.ToList()); // Store updated prices in Redis
        }
    }
}

