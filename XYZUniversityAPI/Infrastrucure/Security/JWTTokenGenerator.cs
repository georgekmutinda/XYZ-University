/*using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;



namespace XYZUniversityAPI.Infrastructure.Security
{
    public class JwtTokenGenerator
    {
        private readonly JwtSettings _settings;

        public JwtTokenGenerator(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateToken(string userId, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
// After Validating the Client id and secret, the next part is to generate a JWT token .The Token includes the clientid.
// the GenerateToken Method takes in the client id and the role as saved in the db. Just hold that thought
// Read appsettings to get the super secret key.
// Cryptography uses bytes so convert the string to bytes form using UTF8 encoding
//Since this key will be used when creating and signing the token we  need to add SymmetricSecurityKey
//Hash Based Message Authentication Code (HMAC) with  secure hashing algorithm(SHA256)  HMAC = (key + Message)Since Both of the 2 systems have the super secret key and the message is same the hash will be the same.If !=key then thats the wrong persin trying..if != message then the message is tampered with
// Create claims(an array),ie the data we want to include in the token payload.Ie the subject,roles,and GuId for the token.
*/
