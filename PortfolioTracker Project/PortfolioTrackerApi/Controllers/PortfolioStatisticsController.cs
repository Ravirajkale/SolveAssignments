using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortfolioTrackerApi.Services;
using System.Security.Claims;

namespace PortfolioTrackerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/statistics")]
    public class PortfolioStatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public PortfolioStatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        //Returns Overall info of user Portfolio's ex:Total invested,Current Value etc
        [HttpGet("portfolio-summary")]
        public async Task<IActionResult> GetPortfolioSummary()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID not found in token.");

            // If your app uses GUIDs for user ID
            if (!Guid.TryParse(userIdClaim, out Guid userId))
                return BadRequest("Invalid User ID format.");

            var summary = await _statisticsService.GetPortfolioSummaryAsync(userIdClaim);
            return Ok(summary);
        }

        [HttpGet("historical-chart/{ticker}")]
        public async Task<IActionResult> GetHistoricalChartData(string ticker, [FromQuery] DateTime? date)
        {
            var data = await _statisticsService.GetHistoricalChartDataAsync(ticker, date);
            return Ok(data);
        }

        //returns chart data for random stocks According to date
        [HttpGet("default-historical-charts/{date}")]
        public async Task<IActionResult> GetDefaultHistoricalCharts(string date)
        {
            var data = await _statisticsService.GetDefaultHistoricalChartDataAsync(date);
            return Ok(data);
        }

        //returns Unique Dates Available
        [HttpGet("available-dates")]
        public async Task<IActionResult> GetAvailableDates()
        {
            var dates = await _statisticsService.GetAvailableChartDatesAsync();
            return Ok(dates); // Returns something like ["2025-04-06", "2025-04-07"]
        }
    }

}
