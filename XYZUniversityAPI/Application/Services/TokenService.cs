using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XYZUniversityAPI.Domain.Entities;
using XYZUniversityAPI.Infrastructure.Security;
using XYZUniversityAPI.Application.DTOs;
using XYZUniversityAPI.Application.Interfaces;

namespace XYZUniversityAPI.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value ?? throw new Exception("JWT configuration is missing.");
        }

        public JwtTokenDto GenerateToken(Client client)
        {
            // 1️⃣ Determine expiration minutes
            int expirationMinutes = client.TokenLifetimeMinutes > 0
                                    ? client.TokenLifetimeMinutes
                                    : _jwtSettings.ExpirationMinutes;

            Console.WriteLine($"JWT ExpirationMinutes used = {expirationMinutes}");

            // 2️⃣ Create signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3️⃣ Add claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, client.ClientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Name", client.ClientName),
                new Claim(ClaimTypes.Role, client.Role)
            };

            // 4️⃣ Compute expiration in UTC
            var utcNow = DateTime.UtcNow;
            var expiresUtc = utcNow.AddMinutes(expirationMinutes);

            // 5️⃣ Create token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: utcNow,
                expires: expiresUtc,
                signingCredentials: creds
            );

            Console.WriteLine($"Token expires at UTC: {token.ValidTo}");
            Console.WriteLine($"Token expires at local: {token.ValidTo.ToLocalTime()}");

            // 6️⃣ Return token DTO
            return new JwtTokenDto
            {
                TokenString = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = token.ValidTo
            };
        }
    }
}
