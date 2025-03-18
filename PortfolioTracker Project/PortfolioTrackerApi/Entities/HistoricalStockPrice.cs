using System.ComponentModel.DataAnnotations;

namespace PortfolioTrackerApi.Entities
{
    public class HistoricalStockPrice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Ticker { get; set; }

        public decimal OpeningPrice { get; set; } // Price at market open
        public decimal ClosingPrice { get; set; } // Price at market close

        public DateTime Date { get; set; } // The date for which these prices are recorded

        //Optional fields
        public decimal HighPrice { get; set; } //The highest price of the ticker for the day
        public decimal LowPrice { get; set; } //The lowest price of the ticker for the day
    }
}
