using StockPriceWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceWorkerService.Repositories
{
    interface IHistoricalStockPriceRepository
    {
        Task AddAsync(HistoricalStockPrice price);
        Task AddRangeAsync(IEnumerable<HistoricalStockPrice> prices);
        Task SaveChangesAsync();
    }
}
