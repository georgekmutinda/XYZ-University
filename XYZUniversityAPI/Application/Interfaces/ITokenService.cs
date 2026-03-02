using XYZUniversityAPI.Domain.Entities;
using XYZUniversityAPI.Application.DTOs;

namespace XYZUniversityAPI.Application.Interfaces
{
    public interface ITokenService
    {
        JwtTokenDto GenerateToken(Client client);
    }
}
