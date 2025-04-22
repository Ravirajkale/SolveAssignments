using StackExchange.Redis;
using StockPriceWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockPriceWorkerService.Services
{
    class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
        }

        public async Task SetStockPricesAsync(List<StockPrice> stocks)
        {
            
            string stockData = JsonSerializer.Serialize(stocks);
            await _db.StringSetAsync("StockPrices", stockData, TimeSpan.FromMinutes(10));
        }

        public async Task<List<StockPrice>> GetStockPricesAsync()
        {
           
            string stockData = await _db.StringGetAsync("StockPrices");

            return !string.IsNullOrEmpty(stockData)
                ? JsonSerializer.Deserialize<List<StockPrice>>(stockData)
                : new List<StockPrice>();
        }

        public async Task PublishAsync(string channel, string message)
        {
            var publisher = _redis.GetSubscriber();
            await publisher.PublishAsync(channel, message);
        }
    }
}
