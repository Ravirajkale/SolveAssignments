using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceWorkerService.Models
{
    class StockPrice
    {
        
        public int Id { get; set; } //Could be ticker for ease

        
        public string Ticker { get; set; }

        public string Company { get; set; }

        public decimal CurrentPrice { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
