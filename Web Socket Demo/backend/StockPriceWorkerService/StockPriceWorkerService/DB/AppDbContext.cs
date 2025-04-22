using Microsoft.EntityFrameworkCore;
using StockPriceWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceWorkerService.DB
{
    class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<StockPrice> StocksPrice { get; set; }
        public DbSet<HistoricalStockPrice> HistoricalStockPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Retains Identity configuration

            // Optionally configure StockPrice relationships or constraints here if needed
        }
    }
}
