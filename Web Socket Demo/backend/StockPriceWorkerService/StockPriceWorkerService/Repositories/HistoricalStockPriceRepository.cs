using StockPriceWorkerService.DB;
using StockPriceWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceWorkerService.Repositories
{
    class HistoricalStockPriceRepository : IHistoricalStockPriceRepository
    {
        private readonly AppDbContext _context;

        public HistoricalStockPriceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(HistoricalStockPrice price)
        {
            await _context.HistoricalStockPrices.AddAsync(price);
        }

        public async Task AddRangeAsync(IEnumerable<HistoricalStockPrice> prices)
        {
            await _context.HistoricalStockPrices.AddRangeAsync(prices);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
