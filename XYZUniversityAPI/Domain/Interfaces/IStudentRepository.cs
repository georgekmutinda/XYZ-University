using XYZUniversityAPI.Domain.Entities;


namespace XYZUniversityAPI.Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student?> GetStudentByAdmissionNumberAsync(string admissionNumber);
        Task<List<Student>> GetAllStudentsAsync();
        Task<bool> StudentExistsAsync(string admissionNumber);
    }
}



//contract for accessing the repository.enfroces abstarction
