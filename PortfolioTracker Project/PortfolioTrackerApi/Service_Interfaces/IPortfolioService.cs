using PortfolioTrackerApi.DTOS;

namespace PortfolioTrackerApi.Service_Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioResponseDto> CreatePortfolioAsync(string userId, PortfolioRequestDtocs portfolioRequest);
        Task<bool> DeleteStockAsync(int stockId);
        Task<(IEnumerable<PortfolioResponseDto>, int)> GetPaginatedPortfoliosAsync(string userId, int pageNumber, int pageSize);
        Task<bool> UpdateQuantityAsync(int stockId, int newQuantity);
    }
}
