using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.DAL;
using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Repositories
{
    public class StocksRepository : IStocksRepository
    {
        private readonly AppDbContext _context;

        public StocksRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockPrice>> GetAllStocksAsync()
            => await _context.StocksPrice.ToListAsync();

        public async Task<StockPrice> GetStockBySymbolAsync(string ticker)
            => await _context.StocksPrice.FirstOrDefaultAsync(s => s.Ticker == ticker);

        public async Task AddStockAsync(StockPrice stock)
        {
            await _context.StocksPrice.AddAsync(stock);
        }

        public async Task UpdateAsync(StockPrice stock)
        {
            _context.StocksPrice.Update(stock);
            await Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddPortfolioStockAsync(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
            await _context.SaveChangesAsync();

        }

        public async Task<List<StockPrice>> GetStocksStartingWith(string query)
        {
            return await _context.StocksPrice
                .Where(s =>
                        s.Ticker.ToLower().Contains(query.ToLower()) ||
                        s.Company.ToLower().Contains(query.ToLower()))
                .OrderBy(s => s.Ticker)
                .Take(15) // Limit to 15 results
                .ToListAsync();
        }
        public async Task<List<string>> GetRandomStockSymbolsAsync(int count)
        {
            return await _context.Stocks
                .OrderBy(x => Guid.NewGuid()) // Random order
                .Select(s => s.Ticker)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Stock>> GetPortfolioStocks(int portfolioid)
        {
            return await _context.Stocks
                .Where(s => s.PortfolioId == portfolioid).ToListAsync();
        }

        public async Task DeleteAsync(Stock stock)
        {
            _context.Stocks.Remove(stock);
            await Task.CompletedTask;
        }

        public async Task<Stock> GetByIdAsync(int stockId)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.Id == stockId);
        }

        public async Task UpdateAsync(Stock stock)
        {
            _context.Stocks.Update(stock);
            await Task.CompletedTask;
        }
    }
}
