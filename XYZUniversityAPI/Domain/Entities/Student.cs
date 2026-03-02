using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XYZUniversityAPI.Domain.Entities
{
    public class Student
    {
        
        public int StudentId { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Admission Number format is invalid.")]
        [Key]
        public string AdmissionNumber { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsValid { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int CourseId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(CreatedBy))]
        public virtual Admin Admin { get; set; } = null!;

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; } = null!;

        public virtual ICollection<StudentContact> Contacts { get; set; }
            = new List<StudentContact>();
    }
}
