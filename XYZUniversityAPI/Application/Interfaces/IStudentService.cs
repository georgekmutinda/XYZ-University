using XYZUniversityAPI.Domain.Entities;
using System.Threading.Tasks;
using XYZUniversityAPI.Application.DTOs;

namespace XYZUniversityAPI.Application.Interfaces
{
    public interface IStudentService
    {
        
        Task<Student?> GetStudentByAdmissionNumberAsync(string AdmissionNumber);

        Task<StudentValidationResponseDto> ValidateStudentAsync(string AdmissionNumber);
    }
}
