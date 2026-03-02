using XYZUniversityAPI.Domain.Entities;
using XYZUniversityAPI.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XYZUniversityAPI.Application.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Student submits payment intent (PENDING)
        /// </summary>
        Task<PaymentInitiatedResponseDto> AddPaymentAsync(PaymentRequestDto paymentRequest);

        /// <summary>
        /// Get all payments for a student
        /// </summary>
        Task<List<PaymentDetailsDto>> GetPaymentsByAdmissionNumberAsync(string admissionNumber);

        /// <summary>
        /// Process bank webhook notification
        /// </summary>
        Task<PaymentResponseDto?> HandleBankNotificationAsync(BankPaymentNotificationDto notification);

        /// <summary>
        /// Lookup all payment types
        /// </summary>
        Task<List<PaymentTypeDto>> GetPaymentTypesAsync();

        /// <summary>
        /// Lookup all payment channels
        /// </summary>
        Task<List<PaymentChannelDto>> GetPaymentChannelsAsync();
    }
}
