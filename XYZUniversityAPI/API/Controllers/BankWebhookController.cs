using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;
using XYZUniversityAPI.Application.DTOs;
using XYZUniversityAPI.Application.Interfaces;

namespace XYZUniversityAPI.API.Controllers
{
    [ApiController]
    [Route("api/bank/webhook")]
    public class BankWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConnectionMultiplexer _redis;
        private readonly XYZUniversityAPI.Application.Interfaces.IRabbitMqPublisher _rabbitMqPublisher;

        private const string RedisPrefix = "WEBHOOK_PAYMENT_";

        public BankWebhookController(
            IPaymentService paymentService,
            IConnectionMultiplexer redis,
            XYZUniversityAPI.Application.Interfaces.IRabbitMqPublisher rabbitMqPublisher)
        {
            _paymentService = paymentService;
            _redis = redis;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        [AllowAnonymous] 
        [HttpPost]
        public async Task<IActionResult> ReceivePaymentNotification([FromBody] BankPaymentNotificationDto notification)
        {
            if (notification == null || string.IsNullOrEmpty(notification.PaymentReference))
                return BadRequest(new { message = "Invalid notification payload." });

            var db = _redis.GetDatabase();
            string redisKey = RedisPrefix + notification.PaymentReference;

            // 1. Idempotency Check
            if (!(await db.StringGetAsync(redisKey)).IsNull)
            {
                return Ok(new { message = "Notification already processed." });
            }

            try
            {
            
                var paymentResponse = await _paymentService.HandleBankNotificationAsync(notification);

                if (paymentResponse == null)
                    return NotFound(new { message = "Reference not found." });

                // 3. Mark as Processed in Redis
                await db.StringSetAsync(redisKey, "PROCESSED", TimeSpan.FromDays(1));

                // 4. Publish to RabbitMQ
                await _rabbitMqPublisher.PublishAsync("payments", paymentResponse);

                Console.WriteLine($"[WEBHOOK] Successfully processed bank notification for {notification.PaymentReference}");
                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error", error = ex.Message });
            }
        }
    }
}
