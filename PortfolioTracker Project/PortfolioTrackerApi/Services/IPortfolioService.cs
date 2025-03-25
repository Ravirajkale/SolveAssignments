using PortfolioTrackerApi.DTOS;

namespace PortfolioTrackerApi.Services
{
    public interface IPortfolioService
    {
        Task<PortfolioResponseDto> CreatePortfolioAsync(string userId, PortfolioRequestDtocs portfolioRequest);
        Task<(IEnumerable<PortfolioResponseDto>, int)> GetPaginatedPortfoliosAsync(string userId, int pageNumber, int pageSize);
    }
}
