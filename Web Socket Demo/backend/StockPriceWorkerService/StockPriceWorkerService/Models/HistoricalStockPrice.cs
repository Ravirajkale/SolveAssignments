using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceWorkerService.Models
{
    class HistoricalStockPrice
    {
        
        public int Id { get; set; }

       
        public string Ticker { get; set; }

        public decimal OpeningPrice { get; set; } // Price at market open
        public decimal ClosingPrice { get; set; } // Price at market close

        public DateTime Date { get; set; } // The date for which these prices are recorded

        //Optional fields
        public decimal HighPrice { get; set; } //The highest price of the ticker for the day
        public decimal LowPrice { get; set; } //The lowest price of the ticker for the day
    }
}
