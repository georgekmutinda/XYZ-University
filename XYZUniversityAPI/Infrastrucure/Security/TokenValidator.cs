using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace XYZUniversityAPI.Infrastructure.Security
{
    public class JwtTokenValidator
    {
        private readonly JwtSettings _settings;

        public JwtTokenValidator(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public bool ValidateToken(string token, out JwtSecurityToken? validatedToken)
        {
            validatedToken = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.SecretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _settings.Audience,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                }, out var securityToken);

                validatedToken = securityToken as JwtSecurityToken;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
