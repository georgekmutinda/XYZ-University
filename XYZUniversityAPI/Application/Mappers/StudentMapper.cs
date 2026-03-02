using XYZUniversityAPI.Domain.Entities; 
using XYZUniversityAPI.Application.DTOs; 

namespace XYZUniversityAPI.Application.Mappers
{
    public static class StudentMapper
    {
        public static StudentValidationResponseDto ToValidationResponseDto(this Student student,decimal courseFee,decimal totalPaid,decimal balance)
        {
            if (student == null) return null!; 

            // Map the student entity to StudentDto.Sevices should return dtos,repo -> entities
            var studentDto = new StudentDto
            {
                AdmissionNumber = student.AdmissionNumber,
                FullName = $"{student.FirstName} {student.LastName}",
                CourseName = student.Course.CourseName,
                CourseFee = courseFee,
                TotalPaid=totalPaid,
                Balance = balance,
                IsActive = student.IsActive,
                IsValid = student.IsValid,
                DateOfBirth = student.DateOfBirth,
                EnrollmentDate = student.EnrollmentDate,
                CreatedBy = student.Admin.AdminName,
                Contacts = student.Contacts.Select(c => new StudentContactDto
                {
                    Email = c.Email,
                    Phone = c.Phone
                }).ToList()
            };

            // Return the full validation response DTO
            return new StudentValidationResponseDto
            {
                Message = student.IsValid ? "Student validation successful." : "Student validation failed.",
                Student = studentDto
            };
        }
    }
}
