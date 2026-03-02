using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; // Added for IServiceProvider
using Microsoft.AspNetCore.SignalR; // Added for HubContext
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using XYZUniversityAPI.Application.DTOs;
using XYZUniversityAPI.Infrastructure.Hubs; 

namespace XYZUniversityAPI.BackgroundServices
{
    public class PaymentNotificationConsumer : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly string _queueName = "payments";
        private readonly ConnectionFactory _factory;
        private readonly ILogger<PaymentNotificationConsumer> _logger;
        private readonly IServiceProvider _serviceProvider; // Added to resolve HubContext

        public PaymentNotificationConsumer(
            IConnectionMultiplexer redis, 
            ILogger<PaymentNotificationConsumer> logger, 
            ConnectionFactory factory,
            IServiceProvider serviceProvider)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _logger = logger;
            _factory = factory;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background worker initializing...");

            using var connection = await _factory.CreateConnectionAsync(stoppingToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: _queueName, 
                durable: true, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null, 
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            
            _logger.LogInformation("Listening for RabbitMQ messages on {Queue}", _queueName);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                try 
                {
                    _logger.LogInformation("Message received from RabbitMQ.");
                    
                    var paymentResponse = JsonSerializer.Deserialize<PaymentResponseDto>(message);
                    if (paymentResponse?.Payment != null)
                    {
                        // 1. Update Redis Cache
                        var db = _redis.GetDatabase();
                        string redisKey = $"STUDENT_PAYMENT_{paymentResponse.Payment.AdmissionNumber}_{paymentResponse.Payment.PaymentId}";
                        await db.StringSetAsync(redisKey, message, TimeSpan.FromDays(1));
                        
                        _logger.LogInformation("Redis cache updated for {Admission}", paymentResponse.Payment.AdmissionNumber);

                        // 2. TRIGGER SIGNALR HUB
                        // We create a scope to get the HubContext safely in a background task
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<PaymentHub>>();
                            
                            // Send "Push" notification to the specific Student Group
                            await hubContext.Clients.Group(paymentResponse.Payment.AdmissionNumber)
                                .SendAsync("ReceivePaymentUpdate", new 
                                { 
                                    PaymentId = paymentResponse.Payment.PaymentId,
                                    Status = paymentResponse.Payment.Status,
                                    Message = $"Success! Your payment of {paymentResponse.Payment.Amount} has been confirmed."
                                }, stoppingToken);
                                Console.WriteLine("Payment updated");
                                
                            _logger.LogInformation("SignalR update pushed to student: {Admission}", paymentResponse.Payment.AdmissionNumber);
                        }
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process RabbitMQ message.");
                }
            };

            await channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
            
            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
