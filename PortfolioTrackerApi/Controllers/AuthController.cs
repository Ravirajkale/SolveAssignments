using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Services;

namespace PortfolioTrackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            var result = await _authService.RegisterAsync(model);

            if (result != null)
            {
                return Ok(result); // Return the JWT token
            }

            return BadRequest("Registration failed."); // Or a more descriptive error
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            var result = await _authService.LoginAsync(model);

            if (result != null)
            {
                return Ok(result); // Return the JWT token
            }

            return Unauthorized("Invalid credentials."); // Or a more descriptive error
        }
    }
}
