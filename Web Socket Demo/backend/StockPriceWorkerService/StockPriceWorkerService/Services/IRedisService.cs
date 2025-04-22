using StockPriceWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceWorkerService.Services
{
    interface IRedisService
    {
        Task SetStockPricesAsync(List<StockPrice> stocks);
        Task<List<StockPrice>> GetStockPricesAsync();
        Task PublishAsync(string channel, string message);
    }
}
