using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.DAL;
using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Repositories
{
    public class HistoricalStockPriceRepository:IHistoricalStockPriceRepository
    {
        private readonly AppDbContext _context;

        public HistoricalStockPriceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<HistoricalStockPrice>> GetByTickerAsync(string ticker)
        {
            return await _context.HistoricalStockPrices
                .Where(p => p.Ticker == ticker)
                .OrderBy(p => p.Date)
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<HistoricalStockPrice> prices)
        {
            await _context.HistoricalStockPrices.AddRangeAsync(prices);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<HistoricalStockPrice>> GetAllAsync()
        {
            return await _context.HistoricalStockPrices
                .ToListAsync();
        }
        public async Task<List<HistoricalStockPrice>> GetByTickerAndDateAsync(string ticker, DateOnly date)
        {
            var startDate = date.ToDateTime(TimeOnly.MinValue);
            var endDate = date.ToDateTime(TimeOnly.MaxValue);

            return await _context.HistoricalStockPrices
                .Where(h => h.Ticker == ticker && h.Date >= startDate && h.Date <= endDate)
                .OrderBy(h => h.Date)
                .ToListAsync();
        }

        public async Task<List<string>> GetRandomTickersByDateAsync(DateOnly date, int count)
        {
            var startDate = date.ToDateTime(TimeOnly.MinValue);
            var endDate = date.ToDateTime(TimeOnly.MaxValue);

            return await _context.HistoricalStockPrices
                .Where(h => h.Date >= startDate && h.Date <= endDate)
                .Select(h => h.Ticker)
                .Distinct()
                .OrderBy(_ => Guid.NewGuid()) // random order
                .Take(count)
                .ToListAsync();
        }
    }
}
