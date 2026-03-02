// File: API/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using XYZUniversityAPI.Application.DTOs;
using XYZUniversityAPI.Application.Interfaces;
using XYZUniversityAPI.Infrastructure.Security;
using XYZUniversityAPI.Domain.Entities;



namespace XYZUniversityAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientService; 

        public AuthController(
            ITokenService tokenService, 
            IConfiguration configuration,
            IClientRepository clientService)
        {
            _tokenService = tokenService;
            _configuration = configuration;
            _clientService = clientService;
        }

       

        [HttpPost("login")]
        public async Task <IActionResult> GenerateToken([FromBody] AuthRequestDto request)
        {
            
            if (string.IsNullOrEmpty(request.ClientName) || string.IsNullOrEmpty(request.ClientSecret))
            {
                return BadRequest(new { Message = "UserName and Password are required." });
            }

         
            var clientFromDb = await _clientService.GetClientByClientNameAsync(request.ClientName);

            if (clientFromDb == null)
            {
                return Unauthorized(new { Message = "User Not Found ." });
            }

            
            var hasher = new PasswordHasher<Client>();
            var result = hasher.VerifyHashedPassword(clientFromDb, clientFromDb.ClientSecretHash, request.ClientSecret);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { Message = "Invalid UserName or Password." });
            }

            
            var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null)
       {
            return StatusCode(500, new { Message = "JWT configuration is missing." });
 }

            var tokenString = _tokenService.GenerateToken(clientFromDb);

            
            var response = new AuthResponseDto
            {
                Token = tokenString.TokenString,
                ExpiresAt = tokenString.ExpiresAt,
                Message = "Token generated successfully."
            };

            // 7️⃣ Return the token and expiry
            return Ok(response);
        }
     

    }
}
