using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Services;
using System.Security.Claims;

namespace PortfolioTrackerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public PortfoliosController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioResponseDto>>> GetPortfolios([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var (portfolios, totalCount) = await _portfolioService.GetPaginatedPortfoliosAsync(userId, pageNumber, pageSize);

            if (totalCount == 0)
                return NotFound("No Portfolios added for this user");
            return Ok(new
            {
                Portfolios = portfolios,
                TotalCount = totalCount
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreatePortfolio([FromBody] PortfolioRequestDtocs PortfolioRequest)
        {
            var userId= User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id is null or invalid");
            }
            if (PortfolioRequest is null || string.IsNullOrEmpty(PortfolioRequest.Name))
                return BadRequest("portfolio name must be valid");

            var PortfolioResponse = await _portfolioService.CreatePortfolioAsync(userId, PortfolioRequest);

            // return CreatedAtAction(nameof(GetPortfolio), new { id = PortfolioResponse.Id }, PortfolioResponse);
            return Ok(PortfolioResponse);
        }
    }
}
