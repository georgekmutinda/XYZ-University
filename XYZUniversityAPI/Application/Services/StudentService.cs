using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using XYZUniversityAPI.Domain.Entities;
using XYZUniversityAPI.Domain.Interfaces;
using XYZUniversityAPI.Application.Interfaces;
using XYZUniversityAPI.Application.DTOs;
using XYZUniversityAPI.Application.Mappers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace XYZUniversityAPI.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IDistributedCache _cache;
        // CHANGED: Use the interface IRabbitMqPublisher instead of the concrete class
        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IPaymentRepository _paymentRepository;

        public StudentService(
            IStudentRepository studentRepository,
            IPaymentRepository paymentRepository,
            IDistributedCache cache,
            IRabbitMqPublisher rabbitMqPublisher)
        {
            _studentRepository = studentRepository;
            _paymentRepository = paymentRepository;
            _cache = cache;
            _rabbitMqPublisher = rabbitMqPublisher;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<Student?> GetStudentByAdmissionNumberAsync(string admissionNumber)
        {
            return await _studentRepository.GetStudentByAdmissionNumberAsync(admissionNumber);
        }

        public async Task<StudentValidationResponseDto> ValidateStudentAsync(string admissionNumber)
        {
            var student = await _studentRepository.GetStudentByAdmissionNumberAsync(admissionNumber);
            if (student == null)
            {
                return new StudentValidationResponseDto
                {
                    Message = $"Student with Admission Number {admissionNumber} not found.",
                    Student = null!
                };
            }

            var courseFee = student.Course.CourseFee;
            
            // FIXED: Removed the extra 's' from Admissison to match the IPaymentRepository interface
            var totalPaid = await _paymentRepository.GetTotalPaidByAdmissionNumberAsync(admissionNumber);
            var balance = courseFee - totalPaid;

            var response = student.ToValidationResponseDto(courseFee, totalPaid, balance);

            // Publish validation event
            await _rabbitMqPublisher.PublishAsync("student_validation_queue", new
            {
                AdmissionNumber = admissionNumber,
                IsValid = student.IsValid,
                Timestamp = DateTime.UtcNow
            });

            var cacheKey = $"student_static_{admissionNumber}";
            var serializedStatic = JsonSerializer.Serialize(new
            {
                student.AdmissionNumber,
                student.FirstName,
                student.LastName,
                student.Course.CourseName,
                student.DateOfBirth,
                student.EnrollmentDate,
                student.Admin.AdminName,
                Contacts = student.Contacts.Select(c => new { c.Email, c.Phone }).ToList()
            }, _jsonOptions);

            await _cache.SetStringAsync(cacheKey, serializedStatic, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });

            return response;
        }
    }
}
