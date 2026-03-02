using Microsoft.AspNetCore.Mvc;
using XYZUniversityAPI.Application.Interfaces;
using XYZUniversityAPI.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace XYZUniversityAPI.API.Controllers
{
    [ApiController]
    [Route("api/student_validation")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/student_validation/validate/
        [Authorize]
        [HttpGet("validate/{admissionNumber}")]
        public async Task<IActionResult> ValidateStudent([FromRoute] string admissionNumber)
        {
            
            var trimmedAdmissionNumber = admissionNumber?.Trim();

            if (string.IsNullOrWhiteSpace(trimmedAdmissionNumber))
            {
                var error = new ApiErrorDto
                {
                    StatusCode = 400,
                    Message = "Admission number cannot be empty or have whitespace.",
                    ErrorType = "ValidationError",
                    Details = "The provided admission number is either null, empty, or contains only whitespace."
                };
                return BadRequest(error);
            }

            var responseDto = await _studentService.ValidateStudentAsync(trimmedAdmissionNumber);

    
            if (responseDto.Student == null)
            {
                var error = new ApiErrorDto
                {
                    StatusCode = 404,
                    Message = "Student not found.",
                    ErrorType = "NotFoundError",
                    Details = $"No student found with admission number: {trimmedAdmissionNumber}"
                };
                return NotFound(error);
            }

           
            if (!responseDto.Student.IsValid || !responseDto.Student.IsActive)
            {
                var reason = !responseDto.Student.IsValid 
                    ? "Transferred or Deferred" 
                    : "Currently Suspended";

                var error = new ApiErrorDto
                {
                    StatusCode = 400, // Using 400 to block the payment operation
                    Message = "Payment process restricted.",
                    ErrorType = "StatusValidationError",
                    Details = $"The Student with admission number: {trimmedAdmissionNumber} is {reason}. Payment is not allowed in this state."
                };
                return BadRequest(error);
            }

            // 4. Detailed checks (kept as found in original code for specific messaging)
            if (responseDto.Student.IsValid == false)
            {
                var error = new ApiErrorDto
                {
                    StatusCode = 404,
                    Message = "StudentId not valid",
                    ErrorType = "NotFoundError",
                    Details = $"The Student with admission number: {trimmedAdmissionNumber} is Transfered or Deferred on the course"
                };
                return NotFound(error);
            }

            if (responseDto.Student.IsActive == false)
            {
                var error = new ApiErrorDto
                {
                    StatusCode = 404,
                    Message = "Inactive student",
                    ErrorType = "NotFoundError",
                    Details = $"Student with admission Number: {trimmedAdmissionNumber} is Currently suspended.Pay fees on resumption to school"
                };
                return NotFound(error);
            }

            // Return full DTO (message + student data) for active/valid students
            return Ok(responseDto);
        }
    }
}
