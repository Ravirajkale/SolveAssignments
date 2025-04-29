using PortfolioTrackerApi.DTOS;

namespace PortfolioTrackerApi.Service_Interfaces
{
    public interface IStatisticsService
    {
        Task<PortfolioSummaryDto> GetPortfolioSummaryAsync(string userId);
        Task<List<HistoricalChartPointDto>> GetHistoricalChartDataAsync(string ticker, DateTime? date = null);
        Task<Dictionary<string, List<HistoricalChartPointDto>>> GetDefaultHistoricalChartDataAsync(string date);
        Task<List<string>> GetAvailableChartDatesAsync();
    }
}
