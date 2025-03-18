using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> RegisterUserAsync(User user, string password);
    }
}
