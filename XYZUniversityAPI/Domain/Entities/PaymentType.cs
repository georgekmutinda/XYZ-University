using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XYZUniversityAPI.Domain.Entities;


namespace XYZUniversityAPI.Domain.Entities
{
    public class PaymentType
    {
        [Key]
        public int PaymentTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; } = null!;

        // Navigation: One PaymentType can have many Payments
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
