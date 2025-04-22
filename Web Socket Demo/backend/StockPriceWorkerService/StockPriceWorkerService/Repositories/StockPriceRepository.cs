using Microsoft.EntityFrameworkCore;
using StockPriceWorkerService.DB;
using StockPriceWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceWorkerService.Repositories
{
    class StockPriceRepository:IStockPriceRepository
    {
        private readonly AppDbContext _context;

        public StockPriceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockPrice>> GetAllAsync()
        {
            return await _context.StocksPrice.ToListAsync();
        }

        public async Task UpdateStockPricesAsync(List<StockPrice> stocks)
        {
            _context.StocksPrice.UpdateRange(stocks);
            await _context.SaveChangesAsync();
        }
    }
}
