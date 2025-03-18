using Microsoft.AspNetCore.Identity;
using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto model);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto model);
    }
}
