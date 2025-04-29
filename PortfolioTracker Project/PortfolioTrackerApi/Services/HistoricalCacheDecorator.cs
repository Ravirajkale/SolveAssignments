using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Repositories;
using PortfolioTrackerApi.Service_Interfaces;
using System.Text.Json;

namespace PortfolioTrackerApi.Services
{
    public class HistoricalCacheDecorator : IHistoricalStockPriceRepository
    {
        private readonly IHistoricalStockPriceRepository _inner;
        private readonly IRedisService _redis;

        public HistoricalCacheDecorator(
            IHistoricalStockPriceRepository inner,
            IRedisService redis)
        {
            _inner = inner;
            _redis = redis;
        }

        public async Task<List<HistoricalStockPrice>> GetByTickerAsync(string ticker)
        {
            var cacheKey = $"HistoricalStockPrices:{ticker}";
            var cached = await _redis.GetValueAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<List<HistoricalStockPrice>>(cached)
                       ?? new List<HistoricalStockPrice>();
            }

            var dbData = await _inner.GetByTickerAsync(ticker);
            if (dbData.Any())
            {
                await _redis.SetValueAsync(cacheKey, JsonSerializer.Serialize(dbData), TimeSpan.FromHours(12));
            }

            return dbData;
        }

        public async Task<List<HistoricalStockPrice>> GetByTickerAndDateAsync(string ticker, DateOnly date)
        {
            var cacheKey = $"HistoricalStockPrices:{ticker}:{date}";
            var cached = await _redis.GetValueAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<List<HistoricalStockPrice>>(cached)
                       ?? new List<HistoricalStockPrice>();
            }

            var dbData = await _inner.GetByTickerAndDateAsync(ticker, date);
            if (dbData.Count!=0)
            {
                
                await _redis.SetValueAsync(cacheKey, JsonSerializer.Serialize(dbData), TimeSpan.FromHours(6));
            }

            return dbData;
        }

        public async Task<List<string>> GetRandomTickersByDateAsync(DateOnly date, int count)
        {
            // This likely doesn’t need caching, just passthrough
            return await _inner.GetRandomTickersByDateAsync(date, count);
        }

        public async Task<List<HistoricalStockPrice>> GetAllAsync()
        {
            // Optional: could cache this too if needed
            return await _inner.GetAllAsync();
        }

        public async Task AddRangeAsync(List<HistoricalStockPrice> prices)
        {
            await _inner.AddRangeAsync(prices);
        }

        public async Task SaveChangesAsync()
        {
            await _inner.SaveChangesAsync();
        }
    }
}