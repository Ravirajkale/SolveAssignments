using System.ComponentModel.DataAnnotations;

namespace PortfolioTrackerApi.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Foreign Key to User

        [Required]
        public string Ticker { get; set; }

        public decimal TargetPrice { get; set; }

        public NotificationType NotificationType { get; set; } // Enum: Email, Push

        public DateTime? SentAt { get; set; } // Nullable - only set when sent

        public User User { get; set; } // Navigation Property
    }
}
