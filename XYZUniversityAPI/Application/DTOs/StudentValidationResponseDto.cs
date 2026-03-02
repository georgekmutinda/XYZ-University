namespace XYZUniversityAPI.Application.DTOs;
public class StudentValidationResponseDto
{
    
    public string Message { get; set; } = null!;
    public StudentDto Student { get; set; } = null!;
}

public class StudentDto
{
    public string AdmissionNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string CourseName { get; set; } = null!;
   public decimal CourseFee { get; set; }
   public decimal TotalPaid { get; set; }
    public decimal Balance {get;set;}
    public bool IsActive { get; set; }
    public bool IsValid { get; set; }
    // add the total fee of a certain course,total amount paid,balance

    public DateTime DateOfBirth { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string CreatedBy { get; set; } = null!;

    public List<StudentContactDto> Contacts { get; set; } = new List<StudentContactDto>();
}


public class StudentContactDto
{
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
}
