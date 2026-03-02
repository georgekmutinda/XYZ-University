namespace XYZUniversityAPI.Application.DTOs
{
    public class JwtTokenDto
    {
        public string TokenString { get; set; } = string.Empty; 
        public DateTime ExpiresAt { get; set; }                 
    }
}
