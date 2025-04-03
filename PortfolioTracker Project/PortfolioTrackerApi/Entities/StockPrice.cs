using System.ComponentModel.DataAnnotations;

namespace PortfolioTrackerApi.Entities
{
    public class StockPrice
    {
        [Key]
        public int Id { get; set; } //Could be ticker for ease

        [Required]
        public string Ticker { get; set; }
        
        public string Company { get; set; }

        public decimal CurrentPrice { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
