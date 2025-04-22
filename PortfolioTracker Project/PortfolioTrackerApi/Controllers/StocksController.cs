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
        private readonly IRedisService redisService;
        public StocksController(IStocksRepository stockRepository, StocksService stockService, IRedisService redisService)
        {
            _stockRepository = stockRepository;
            _stockService = stockService;
            this.redisService = redisService;
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
            Console.WriteLine(ticker);
            if (stock == null) return NotFound("Stock not found.");
            return Ok(stock);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStocks(string query)
      {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty.");

            var matchingStocks = await _stockRepository.GetStocksStartingWith(query);

            var result = new List<StockPriceDto>();

            foreach (var stock in matchingStocks)
            {
                var priceInfo = await redisService.GetPriceAsync(stock.Ticker);
                result.Add(new StockPriceDto
                {
                    Ticker = stock.Ticker,
                    Company = stock.Name,
                    CurrentPrice = priceInfo?.CurrentPrice ?? 0,
                    LastUpdated = priceInfo?.LastUpdated ?? DateTime.UtcNow
                });
            }
          
            return Ok(result);
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
                return Empty;
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
