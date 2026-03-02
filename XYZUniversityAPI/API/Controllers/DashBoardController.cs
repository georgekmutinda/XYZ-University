using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;
using System.Text.Json;
using XYZUniversityAPI.Application.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XYZUniversityAPI.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;
        public DashboardController(IConnectionMultiplexer redis) => _redis = redis;

        [Authorize] 
        [HttpGet("live-payments")]
        public async Task<IActionResult> GetLivePayments()
        {
           
            if (!User.IsInRole("Admin"))
            {
                return StatusCode(403, new ApiErrorDto
                {
                    StatusCode = 403,
                    Message = "Access Denied",
                    ErrorType = "AuthorizationError",
                    Details = "Only Admins can view this resource."
                });
            }

            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            
            var keys = server.Keys(pattern: "STUDENT_PAYMENT_*").ToList();
            var liveStats = new List<PaymentDetailsDto>();

            foreach (var key in keys)
            {
                var data = await db.StringGetAsync(key);
                if (!data.IsNull)
                {
                    var response = JsonSerializer.Deserialize<PaymentResponseDto>(data!);
                    if (response?.Payment != null)
                    {
                        liveStats.Add(response.Payment);
                    }
                }
            }

            return Ok(new
            {
                TotalLiveTracked = liveStats.Count,
                SuccessCount = liveStats.Count(p => p.Status == "SUCCESS"),
                PendingCount = liveStats.Count(p => p.Status == "PENDING"),
                Payments = liveStats
            });
        }
    }
}
