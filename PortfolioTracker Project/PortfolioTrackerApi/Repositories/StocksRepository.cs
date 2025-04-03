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


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddPortfolioStockAsync(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
            await _context.SaveChangesAsync();

        }
    }
}
