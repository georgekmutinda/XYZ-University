using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using XYZUniversityAPI.Application.Interfaces;
using XYZUniversityAPI.Application.DTOs;
using XYZUniversityAPI.Application.Mappers;
using XYZUniversityAPI.Application.Services;
using StackExchange.Redis;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System;

namespace XYZUniversityAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IStudentService _studentService;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IPaymentService paymentService, 
            IStudentService studentService, 
            IConnectionMultiplexer redis,
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _studentService = studentService;
            _redis = redis;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("types")]
        public async Task<IActionResult> GetPaymentTypes()
        {
            var types = await _paymentService.GetPaymentTypesAsync();
            return (types == null || !types.Any()) ? NotFound() : Ok(types);
        }

        [Authorize]
        [HttpGet("channels")]
        public async Task<IActionResult> GetPaymentChannels()
        {
            var channels = await _paymentService.GetPaymentChannelsAsync();
            return (channels == null || !channels.Any()) ? NotFound() : Ok(channels);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] PaymentRequestDto paymentRequest)
        {
            if (paymentRequest == null) return BadRequest("Invalid payload.");
            
            paymentRequest.AdmissionNumber = Regex.Replace(paymentRequest.AdmissionNumber ?? "", @"\s+", "");

            var db = _redis.GetDatabase();
            // 🔒 REDIS DISTRIBUTED LOCK: Prevent duplicate intent for Student + Amount
            string lockKey = $"PAYMENT_LOCK_{paymentRequest.AdmissionNumber}_{paymentRequest.Amount}";
            
            bool isLocked = await db.StringSetAsync(lockKey, "LOCKED", TimeSpan.FromMinutes(5), When.NotExists);

            if (!isLocked)
            {
                _logger.LogWarning("Duplicate payment attempt blocked for {Admission}", paymentRequest.AdmissionNumber);
                return Conflict(new { message = "A payment for this amount is already in progress. Please wait." });
            }

            try 
            {
                if (paymentRequest.Amount <= 0)
                {
                    await db.KeyDeleteAsync(lockKey);
                    return BadRequest("Amount must be > 0.");
                }

                var student = await _studentService.GetStudentByAdmissionNumberAsync(paymentRequest.AdmissionNumber);
                if (student == null || !student.IsValid)
                {
                    await db.KeyDeleteAsync(lockKey);
                    return NotFound("Student not found or inactive.");
                }
              
                var response = await _paymentService.AddPaymentAsync(paymentRequest);
                return Accepted(response);
            }
            catch (Exception ex)
            {
                await db.KeyDeleteAsync(lockKey);
                _logger.LogError(ex, "Error initiating payment.");
                return StatusCode(500, "Internal error.");
            }
        }
        
        [Authorize]
        [HttpGet("payment-latest-status")]
        public async Task<IActionResult> GetMyLatestPaymentStatus([FromQuery] string admissionNumber)
        {
            if (string.IsNullOrWhiteSpace(admissionNumber)) return BadRequest("Admission Number required.");

            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            
            var keys = server.Keys(pattern: $"STUDENT_PAYMENT_{admissionNumber.Trim()}_*").ToList();
            PaymentDetailsDto? latestPayment = null;

            // 1. Try Cache
            if (keys.Any())
            {
                var latestKey = keys.OrderByDescending(k => k.ToString()).First();
                var cachedValue = await db.StringGetAsync(latestKey);
                
                if (cachedValue.HasValue)
                {
                    var cachedResponse = JsonSerializer.Deserialize<PaymentResponseDto>(cachedValue.ToString()!);
                    latestPayment = cachedResponse?.Payment;
                }
            }

            // 2. Try DB if cache is empty
            if (latestPayment == null)
            {
                var dbPayments = await _paymentService.GetPaymentsByAdmissionNumberAsync(admissionNumber);
                latestPayment = dbPayments.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
            }

            if (latestPayment != null) 
            {
               
                return Ok(new 
                {
                    admissionNumber = latestPayment.AdmissionNumber,
                    amount = latestPayment.Amount,
                    status = latestPayment.Status,
                    bankReference = latestPayment.BankReference,
                    paymentType = latestPayment.PaymentType,
                    paymentChannel = latestPayment.PaymentChannel,
                    createdAt = latestPayment.CreatedAt
                });
            }

            return NotFound("No payment history found.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all_payments")]
        public async Task<IActionResult> GetPaymentsByAdmissionNumber([FromQuery] string admissionNumber)
        {
            if (string.IsNullOrWhiteSpace(admissionNumber)) return BadRequest("Admission Number required.");

            var dbPayments = await _paymentService.GetPaymentsByAdmissionNumberAsync(admissionNumber);

            if (dbPayments == null || !dbPayments.Any()) return NotFound("No records found.");

            // Admin gets full details including internal messages
            var responseList = dbPayments
                .Select(p => p.ToPaymentResponseDto("Verified"))
                .ToList();

            return Ok(new { 
                count = responseList.Count, 
                payments = responseList 
            });
        }
    }
}
