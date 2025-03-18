using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioTrackerApi.Services
{
    public class AuthService:IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration; //For settings

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto model)
        {
            // Create a new User entity
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            // Register the user using the repository
            bool registrationSuccessful = await _userRepository.RegisterUserAsync(user, model.Password);

            if (registrationSuccessful)
            {
                
                
                return new AuthResponseDto { Token = "Registration Succesfull" };
            }

            // Registration failed
            return null; // Or throw an exception
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto model)
        {
            // Retrieve the user by email
            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            // Check if the user exists and the password is correct
            if (user != null && await CheckPasswordAsync(user, model.Password))
            {
                // Generate a JWT token for the logged-in user
                string token = GenerateJwtToken(user);
                return new AuthResponseDto { Token = token };
            }

            // Login failed
            return null; // Or throw an exception
        }

        private string GenerateJwtToken(User user)
        {
            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),  // User ID
                new Claim(ClaimTypes.Email, user.Email),
               
            };

            //Get the Secret Key
            string secretKey = _configuration["JwtSettings:SecretKey"];

            // Create a signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Create signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create a JWT security token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Set the expiration time
                signingCredentials: creds
            );

            // Serialize the token to a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await Task.FromResult(new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success);
        }
    }
}
