using PortfolioTrackerApi.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace PortfolioTrackerApi.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis; //Singleton
        public RedisService(IConfiguration configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
        }

        public async Task<string> GetValueAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task SetValueAsync(string key, string value, TimeSpan expiry)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, value, expiry);
        }
        public async Task SetStockPricesAsync(List<StockPrice> stocks)
        {
            var db = _redis.GetDatabase();
            string stockData = JsonSerializer.Serialize(stocks);
            await db.StringSetAsync("StockPrices", stockData, TimeSpan.FromMinutes(10)); // Store in Redis for 10 mins
        }

        public async Task<List<StockPrice>> GetStockPricesAsync()
        {
            var db = _redis.GetDatabase();
            string stockData = await db.StringGetAsync("StockPrices");
            return stockData != null ? JsonSerializer.Deserialize<List<StockPrice>>(stockData) : new List<StockPrice>();
        }

        
    }

}
