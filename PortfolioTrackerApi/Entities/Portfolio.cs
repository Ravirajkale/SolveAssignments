using System.ComponentModel.DataAnnotations;

namespace PortfolioTrackerApi.Entities
{
    public class Portfolio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; } // Foreign Key to User (IdentityUser.Id)

        public User User { get; set; } // Navigation Property

        public List<Stock> Stocks { get; set; } = new List<Stock>(); // Navigation Property
    }
}