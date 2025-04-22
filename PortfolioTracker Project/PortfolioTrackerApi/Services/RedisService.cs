using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace PortfolioTrackerApi.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis; //Singleton
        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
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

        public async Task<StockPriceDto?> GetPriceAsync(string symbol)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync("StockPrices");

            if (!value.HasValue)
                return null; // No data in Redis

            var stockList = JsonSerializer.Deserialize<List<StockPriceDto>>(value);

            return stockList?.FirstOrDefault(s => s.Ticker.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        }
        public ISubscriber GetSubscriber()
        {
            return _redis.GetSubscriber();
        }
        public async Task<List<HistoricalStockPrice>> GetHistoricalPricesAsync(string ticker)
        {
            var db = _redis.GetDatabase();
            string cacheKey = $"HistoricalStockPrices:{ticker}";
            string? cachedData = await db.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<List<HistoricalStockPrice>>(cachedData) ?? new List<HistoricalStockPrice>();
            }

            return new List<HistoricalStockPrice>();
        }

        public async Task SetHistoricalPricesAsync(string ticker, List<HistoricalStockPrice> prices)
        {
            var db = _redis.GetDatabase();
            string cacheKey = $"HistoricalStockPrices:{ticker}";
            string data = JsonSerializer.Serialize(prices);
            await db.StringSetAsync(cacheKey, data, TimeSpan.FromHours(6)); // Cache for 6 hours
        }
        public async Task<List<HistoricalStockPrice>> GetHistoricalPricesByDateAsync(string ticker, DateOnly date)
        {
            var db = _redis.GetDatabase();
            string cacheKey = $"HistoricalStockPrices:{ticker}:{date}";
            var data = await db.StringGetAsync(cacheKey);

            return !string.IsNullOrEmpty(data)
                ? JsonSerializer.Deserialize<List<HistoricalStockPrice>>(data) ?? new List<HistoricalStockPrice>()
                : new List<HistoricalStockPrice>();
        }

        public async Task SetHistoricalPricesByDateAsync(string ticker, DateOnly date, List<HistoricalStockPrice> prices)
        {
            var db = _redis.GetDatabase();
            string cacheKey = $"HistoricalStockPrices:{ticker}:{date}";
            string serialized = JsonSerializer.Serialize(prices);
            await db.StringSetAsync(cacheKey, serialized, TimeSpan.FromHours(6)); // optional: vary expiry
        }
    }

}
