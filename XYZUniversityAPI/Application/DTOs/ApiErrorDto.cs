namespace XYZUniversityAPI.Application.DTOs
{
    public class ApiErrorDto
    {
         public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
       
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string ErrorType { get; set; } = string.Empty;
       
        public string? Details { get; set; }
    }
}