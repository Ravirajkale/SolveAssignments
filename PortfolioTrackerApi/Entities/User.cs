using Microsoft.AspNetCore.Identity;

namespace PortfolioTrackerApi.Entities
{
    public class User : IdentityUser
    {
        // IdentityUser already includes: Id, UserName, Email, PasswordHash, etc.
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Portfolio> Portfolios { get; set; } = new List<Portfolio>(); // Navigation Property
    }

}