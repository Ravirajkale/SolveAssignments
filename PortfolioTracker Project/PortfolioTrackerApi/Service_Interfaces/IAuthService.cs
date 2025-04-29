using Microsoft.AspNetCore.Identity;
using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Service_Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto model);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto model);
    }
}
