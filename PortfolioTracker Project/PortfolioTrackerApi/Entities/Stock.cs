using System.ComponentModel.DataAnnotations;

namespace PortfolioTrackerApi.Entities
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PortfolioId { get; set; } // Foreign Key to Portfolio

        [Required]
        public string Name { get; set; }  // E.g., "Apple Inc."

        [Required]
        public string Ticker { get; set; } // E.g., "AAPL"

        public int Quantity { get; set; }

        [Required]
        public decimal PurchasePrice { get; set; }

        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow; // Default to now

        public Portfolio Portfolio { get; set; } // Navigation Property
    }
}