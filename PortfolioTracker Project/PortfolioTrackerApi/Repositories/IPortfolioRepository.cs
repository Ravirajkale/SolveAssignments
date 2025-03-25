using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Repositories
{
    public interface IPortfolioRepository
    {
        Task AddPortfolioAsync(Portfolio entity);
        Task<(IEnumerable<PortfolioResponseDto>, int)> GetPaginatedPortfoliosAsync(string userId, int pageNumber, int pageSize);
    }
}
