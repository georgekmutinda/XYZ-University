// Application/Interfaces/IClientRepository.cs
using XYZUniversityAPI.Domain.Entities;

namespace XYZUniversityAPI.Application.Interfaces
{
    public interface IClientRepository
    {
        Task<Client?> GetClientByClientNameAsync(string clientName);
    }
}
