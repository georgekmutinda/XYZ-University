using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // Added
using System.Net.Http.Json;
using XYZUniversityAPI.Application.DTOs;
using XYZUniversityAPI.Application.Interfaces;
using XYZUniversityAPI.Application.Mappers;
using XYZUniversityAPI.Domain.Entities;
using XYZUniversityAPI.Domain.Interfaces;

namespace XYZUniversityAPI.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PaymentService> _logger; // Added

        public PaymentService(
            IPaymentRepository paymentRepository, 
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PaymentService> logger) 
        {
            _paymentRepository = paymentRepository;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<PaymentInitiatedResponseDto> AddPaymentAsync(PaymentRequestDto paymentRequest)
        {
            if (paymentRequest == null) throw new ArgumentNullException(nameof(paymentRequest));

            _logger.LogInformation("Processing new payment intent for {Admission}", paymentRequest.AdmissionNumber);

            var payment = paymentRequest.ToPaymentEntity();
            await _paymentRepository.AddPaymentAsync(payment);

            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";

            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogDebug("Starting 3s delay for Simulator: {Ref}", payment.ReferenceNumber);
                    await Task.Delay(3000); 

                    using var client = _httpClientFactory.CreateClient();
                    
                    var simulationPayload = new BankPaymentNotificationDto
                    {
                        PaymentReference = payment.ReferenceNumber,
                        BankReference = "SIM-BANK-" + Guid.NewGuid().ToString("N").ToUpper().Substring(0, 8),
                        Status = "SUCCESS",
                        PaidBy = payment.PaidBy,
                        AmountPaid = payment.Amount 
                    };

                    _logger.LogInformation("Simulator loopback initiated for {Ref}", payment.ReferenceNumber);
                    var response = await client.PostAsJsonAsync($"{baseUrl}/api/bank/webhook", simulationPayload);
                    
                    if (response.IsSuccessStatusCode)
                        _logger.LogInformation("[SIMULATOR SUCCESS] Webhook hit for {Ref}", payment.ReferenceNumber);
                    else
                        _logger.LogWarning("[SIMULATOR FAILED] Webhook status: {Code}", response.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Simulator crashed for {Ref}", payment.ReferenceNumber);
                }
            });

            return new PaymentInitiatedResponseDto
            {
                Message = "Payment initiated. Awaiting bank confirmation.",
                Payment = new PaymentPendingDto
                {
                    AdmissionNumber = payment.AdmissionNumber,
                    Amount = payment.Amount,
                    Status = "PENDING",
                    CreatedAt = payment.CreatedAt
                }
            };
        }

        public async Task<PaymentResponseDto?> HandleBankNotificationAsync(BankPaymentNotificationDto notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            _logger.LogInformation("Webhook hit for Reference: {Ref}", notification.PaymentReference);

            var payment = await _paymentRepository.GetPaymentByReferenceAsync(notification.PaymentReference);
            if (payment == null) return null;

            if (payment.Status != PaymentStatus.PENDING) 
                return payment.ToPaymentResponseDto($"Already processed: {payment.Status}");

            payment.Status = notification.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) 
                             ? PaymentStatus.SUCCESS 
                             : PaymentStatus.FAILED;
            
            payment.BankReference = notification.BankReference;
            payment.PaymentDate = DateTime.UtcNow;

            await _paymentRepository.UpdatePaymentAsync(payment);

            _logger.LogInformation("Payment {Ref} updated to {Status}", payment.ReferenceNumber, payment.Status);

            return payment.ToPaymentResponseDto(payment.Status.ToString());
        }

        public async Task<List<PaymentDetailsDto>> GetPaymentsByAdmissionNumberAsync(string admissionNumber)
        {
            _logger.LogInformation("Querying DB for all payments of {Admission}", admissionNumber);
            var payments = await _paymentRepository.GetPaymentsByAdmissionNumberAsync(admissionNumber);
            return payments?.Select(PaymentMapper.ToPaymentDetailsDto).ToList() ?? new List<PaymentDetailsDto>();
        }

        public async Task<List<PaymentTypeDto>> GetPaymentTypesAsync() => (await _paymentRepository.GetPaymentTypesAsync()).Select(pt => new PaymentTypeDto { PaymentTypeId = pt.PaymentTypeId, TypeName = pt.TypeName }).ToList();
        public async Task<List<PaymentChannelDto>> GetPaymentChannelsAsync() => (await _paymentRepository.GetPaymentChannelsAsync()).Select(pc => new PaymentChannelDto { PaymentChannelId = pc.PaymentChannelId, ChannelName = pc.ChannelName }).ToList();
    }
}
