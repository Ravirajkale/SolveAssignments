using System.ComponentModel.DataAnnotations;

namespace PortfolioTrackerApi.DTOS
{
    public class StockPriceDto
    {
     
        public string Ticker { get; set; }

        public string Company { get; set; }

        public decimal CurrentPrice { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
