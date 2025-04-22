using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Repositories
{
    public interface IPortfolioRepository
    {
        Task AddPortfolioAsync(Portfolio entity);
        Task<(IEnumerable<PortfolioResponseDto>, int)> GetPaginatedPortfoliosAsync(string userId, int pageNumber, int pageSize);
        Task<Portfolio> GetPortfolioByIdAsync(int portfolioId);
        Task<Portfolio> GetPortfolioStocksAsync(int portfolioId);
        Task<IEnumerable<Portfolio>> GetUserPortfoliosWithStocksAsync(string userId);

       // Task<List<Portfolio>> GetAllPortfoliosWithStocksAsync();
    }
}
