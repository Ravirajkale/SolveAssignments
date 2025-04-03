using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Repositories;
using PortfolioTrackerApi.Services;

namespace PortfolioTrackerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStocksRepository _stockRepository;
        private readonly StocksService _stockService;

        public StocksController(IStocksRepository stockRepository, StocksService stockService)
        {
            _stockRepository = stockRepository;
            _stockService = stockService;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableStocks()
        {
            var stocks = await _stockService.GetAvailableStocksAsync();
            return Ok(stocks);
        }

        [HttpGet("search/{ticker}")]
        public async Task<IActionResult> SearchStock(string ticker)
        {
            var stock = await _stockService.GetStockWithCacheAsync(ticker);
            if (stock == null) return NotFound("Stock not found.");
            return Ok(stock);
        }

        [HttpGet("{portfolioId}/stocks")]
        public async Task<IActionResult> GetPortfolioStocks(int portfolioId)
        {
            try
            {
                var portfolioStocks = await _stockService.GetPortfolioStocksAsync(portfolioId);
                return Ok(portfolioStocks);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddStock([FromBody] StockAddDto stockDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _stockService.AddStockToPortfolioAsync(stockDto);
                return Ok(new { message = "Stock added successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }


}
