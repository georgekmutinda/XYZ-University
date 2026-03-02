
namespace XYZUniversityAPI.Domain.Entities
{
    public class Client
    { 

        
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecretHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int TokenLifetimeMinutes { get; set; } = 60;
         public string ClientName { get; set; } = string.Empty;
         public string Role { get; set; } = "User";
        public string? ContactEmail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system";
        
    }
}