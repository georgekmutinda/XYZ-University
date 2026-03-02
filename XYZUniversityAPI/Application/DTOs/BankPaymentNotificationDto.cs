namespace XYZUniversityAPI.Application.DTOs
{
    public class BankPaymentNotificationDto
    {
        public string PaymentReference { get; set; } = null!; // internal system reference
        public string? BankReference { get; set; } // reference from bank
        public decimal AmountPaid { get; set; }
        public string Status { get; set; } = null!; // SUCCESS / FAILED
        public string PaidBy { get; set; } = null!; // optional
    }
}
