using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Repositories;

namespace PortfolioTrackerApi.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository portfolioRepository;

        public PortfolioService(IPortfolioRepository portfolioRepository)
        {
            this.portfolioRepository = portfolioRepository;
        }

        public async Task<PortfolioResponseDto> CreatePortfolioAsync(string userId, PortfolioRequestDtocs portfolioRequest)
        {
            var entity = new Portfolio()
            {
                Name=portfolioRequest.Name,
                UserId = userId

            };

            await portfolioRepository.AddPortfolioAsync(entity);

            return new PortfolioResponseDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public async Task<(IEnumerable<PortfolioResponseDto>, int)> GetPaginatedPortfoliosAsync(string userId, int pageNumber, int pageSize)
        {
            var (portfolios, totalCount) = await portfolioRepository.GetPaginatedPortfoliosAsync(userId, pageNumber, pageSize);

            return (portfolios, totalCount);

        }
    }
}
