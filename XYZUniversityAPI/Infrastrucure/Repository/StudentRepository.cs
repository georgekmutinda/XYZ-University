using Microsoft.EntityFrameworkCore;
using XYZUniversityAPI.Domain.Entities;
using XYZUniversityAPI.Domain.Interfaces;
using XYZUniversityAPI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XYZUniversityAPI.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _dbContext;

        public StudentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Get a student by AdmissionNumber (PK) including related data for validation
        /// </summary>
        public async Task<Student?> GetStudentByAdmissionNumberAsync(string admissionNumber)
        {
            if (string.IsNullOrWhiteSpace(admissionNumber))
                throw new ArgumentException("Admission number cannot be null or empty", nameof(admissionNumber));

            return await _dbContext.Students
                .Include(s => s.Course)
                .Include(s => s.Admin)
                .Include(s => s.Contacts)
                .FirstOrDefaultAsync(s => s.AdmissionNumber == admissionNumber);
        }

        /// <summary>
        /// Get all students including related entities
        /// </summary>
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _dbContext.Students
                .Include(s => s.Course)
                .Include(s => s.Admin)
                .Include(s => s.Contacts)
                .ToListAsync();
        }

        /// <summary>
        /// Optional: Check if a student exists by AdmissionNumber
        /// Useful for validation without fetching full entity
        /// </summary>
        public async Task<bool> StudentExistsAsync(string admissionNumber)
        {
            if (string.IsNullOrWhiteSpace(admissionNumber))
                throw new ArgumentException("Admission number cannot be null or empty", nameof(admissionNumber));

            return await _dbContext.Students
                .AnyAsync(s => s.AdmissionNumber == admissionNumber);
        }
    }
}
