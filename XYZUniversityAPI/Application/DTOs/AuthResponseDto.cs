// File: Application/DTOs/AuthResponseDto.cs
namespace XYZUniversityAPI.Application.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; } = "Message:Authentication successful.";
    }
}
