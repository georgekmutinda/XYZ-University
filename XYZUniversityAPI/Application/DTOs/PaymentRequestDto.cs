using System.ComponentModel.DataAnnotations;

namespace XYZUniversityAPI.Application.DTOs
{
    public class PaymentRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string AdmissionNumber { get; set; } = null!;

        [Required]
        [Range(1.0, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(150)]
        public string PaidBy { get; set; } = null!;

        [Required]
        public int PaymentTypeId { get; set; } 

        [Required]
        public int PaymentChannelId { get; set; } 
    }
}
