using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XYZUniversityAPI.Domain.Entities
{
    public class PaymentChannel
    {
        [Key]
        public int PaymentChannelId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ChannelName { get; set; } = null!;

        // Navigation: One channel can be used for many payments
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
