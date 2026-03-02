
using XYZUniversityAPI.Application.Interfaces;
using XYZUniversityAPI.Domain.Entities;


namespace XYZUniversityAPI.Application.Services
{
    public class ClientService : IClientRepository
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Client?> GetClientByClientNameAsync(string clientName)
        {
            return await _clientRepository.GetClientByClientNameAsync(clientName);
        }
    }
}
