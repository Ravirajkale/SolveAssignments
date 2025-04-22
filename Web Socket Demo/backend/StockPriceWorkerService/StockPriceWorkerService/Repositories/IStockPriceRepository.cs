using StockPriceWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceWorkerService.Repositories
{
    interface IStockPriceRepository
    {
        Task<List<StockPrice>> GetAllAsync();
        Task UpdateStockPricesAsync(List<StockPrice> stocks);
    }
}
