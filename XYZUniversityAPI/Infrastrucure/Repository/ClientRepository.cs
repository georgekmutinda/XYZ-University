// Infrastructure/Repositories/ClientRepository.cs
using Microsoft.EntityFrameworkCore;
using XYZUniversityAPI.Application.Interfaces;
using XYZUniversityAPI.Domain.Entities;
using XYZUniversityAPI.Infrastructure.Data;

namespace XYZUniversityAPI.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _dbContext;

        public ClientRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Client?> GetClientByClientNameAsync(string clientName)
        {
            return await _dbContext.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClientName  == clientName);
        }
    }
}
