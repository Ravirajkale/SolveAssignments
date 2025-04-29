using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Service_Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace PortfolioTrackerApi.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis; // Master
        private IConnectionMultiplexer? _replicaConnection;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        private async Task<IDatabase> GetReplicaDatabaseAsync()
        {
            _replicaConnection ??= await ConnectionMultiplexer.ConnectAsync("localhost:6380");
            return _replicaConnection.GetDatabase();
        }

        private async Task<T> ExecuteWithFallbackAsync<T>(Func<IDatabase, Task<T>> redisAction, bool isWrite = false)
        {
            var masterDb = _redis.GetDatabase();
            try
            {
                return await redisAction(masterDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Primary Redis failed: {ex.Message}.");

                if (isWrite)
                {
                    Console.WriteLine("❌ Write operation skipped on replica.");
                    return default!;
                }

                Console.WriteLine("Falling back to replica for read...");
                var replicaDb = await GetReplicaDatabaseAsync();
                return await redisAction(replicaDb);
            }
        }


        private async Task ExecuteWithFallbackAsync(Func<IDatabase, Task> redisAction,bool isWrite=false)
        {
            var masterDb = _redis.GetDatabase();
            try
            {
                await redisAction(masterDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Primary Redis failed: {ex.Message}. Falling back to replica...");

                if(isWrite)
                {
                    Console.WriteLine("Write Operation skipped on replica");
                    return;
                }
                var replicaDb = await GetReplicaDatabaseAsync();
                await redisAction(replicaDb);
            }
        }

        public Task<string> GetValueAsync(string key)
        { 
            return ExecuteWithFallbackAsync(async db =>
            {
                RedisValue redisValue = await db.StringGetAsync(key);
                return redisValue.ToString();
            });
          
        }

        public Task SetValueAsync(string key, string value, TimeSpan expiry) =>
            ExecuteWithFallbackAsync(db => db.StringSetAsync(key, value, expiry), isWrite: true);

        public Task SetStockPricesAsync(List<StockPrice> stocks)
        {
            string data = JsonSerializer.Serialize(stocks);
            return ExecuteWithFallbackAsync(db => db.StringSetAsync("StockPrices", data, TimeSpan.FromMinutes(10)), isWrite: true);
        }

        public async Task<List<StockPrice>> GetStockPricesAsync()
        {
            string? data = await ExecuteWithFallbackAsync(db => db.StringGetAsync("StockPrices"));
            return !string.IsNullOrEmpty(data) ? JsonSerializer.Deserialize<List<StockPrice>>(data) ?? new() : new();
        }

        public async Task<StockPriceDto?> GetPriceAsync(string symbol)
        {
            string? data = await ExecuteWithFallbackAsync(db => db.StringGetAsync("StockPrices"));
            var list = JsonSerializer.Deserialize<List<StockPriceDto>>(data ?? "");
            return list?.FirstOrDefault(s => s.Ticker.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        }

        public ISubscriber GetSubscriber() => _redis.GetSubscriber();

        public async Task<List<HistoricalStockPrice>> GetHistoricalPricesAsync(string ticker)
        {
            string key = $"HistoricalStockPrices:{ticker}";
            string? data = await ExecuteWithFallbackAsync(db => db.StringGetAsync(key));
            return !string.IsNullOrEmpty(data) ? JsonSerializer.Deserialize<List<HistoricalStockPrice>>(data) ?? new() : new();
        }

        public Task SetHistoricalPricesAsync(string ticker, List<HistoricalStockPrice> prices)
        {
            string key = $"HistoricalStockPrices:{ticker}";
            string data = JsonSerializer.Serialize(prices);
            return ExecuteWithFallbackAsync(db => db.StringSetAsync(key, data, TimeSpan.FromHours(6)), isWrite: true);
        }

        public async Task<List<HistoricalStockPrice>> GetHistoricalPricesByDateAsync(string ticker, DateOnly date)
        {
            string key = $"HistoricalStockPrices:{ticker}:{date}";
            string? data = await ExecuteWithFallbackAsync(db => db.StringGetAsync(key));
            return !string.IsNullOrEmpty(data) ? JsonSerializer.Deserialize<List<HistoricalStockPrice>>(data) ?? new() : new();
        }

        public Task SetHistoricalPricesByDateAsync(string ticker, DateOnly date, List<HistoricalStockPrice> prices)
        {
            string key = $"HistoricalStockPrices:{ticker}:{date}";
            string data = JsonSerializer.Serialize(prices);
            return ExecuteWithFallbackAsync(db => db.StringSetAsync(key, data, TimeSpan.FromHours(6)), isWrite: true);
        }
    }
}
