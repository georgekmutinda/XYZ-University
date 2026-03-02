using System;

namespace XYZUniversityAPI.Application.DTOs
{
    public class PaymentResponseDto
    {
        public string Message { get; set; } = string.Empty;

        public PaymentDetailsDto Payment { get; set; } = null!;
    }

    public class PaymentDetailsDto
    {
       public int PaymentId { get; set; }

        public string AdmissionNumber { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Status { get; set; } = null!; 

        public string? BankReference { get; set; } 

        public string PaymentType { get; set; } = null!;

        public string PaymentChannel { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
    public class PaymentPendingDto
    {
        public string AdmissionNumber { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime CreatedAt { get; set; }
        
    }
    public class PaymentInitiatedResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public PaymentPendingDto Payment { get; set; } = null!;
        
    }
}
