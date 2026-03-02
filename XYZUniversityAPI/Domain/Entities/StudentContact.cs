using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XYZUniversityAPI.Domain.Entities
{
    public class StudentContact
    {
        [Key]
        public int StudentContactId { get; set; }

        [Required]
        [StringLength(20)]
        public string AdmissionNumber { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15)]
        public string Phone { get; set; } = null!;

        

        [ForeignKey(nameof(AdmissionNumber))]
        public virtual Student Student { get; set; } = null!;
    }
}
