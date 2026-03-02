using XYZUniversityAPI.Application.DTOs;
using XYZUniversityAPI.Domain.Entities;
using System;

namespace XYZUniversityAPI.Application.Mappers
{
    public static class PaymentMapper
    {
      
        public static Payment ToPaymentEntity(this PaymentRequestDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Payment
            {
                AdmissionNumber = dto.AdmissionNumber,
                Amount = dto.Amount,
                PaidBy = dto.PaidBy,
                PaymentTypeId = dto.PaymentTypeId,
                PaymentChannelId = dto.PaymentChannelId,
                Status = PaymentStatus.PENDING,
                
                CreatedAt = DateTime.UtcNow, 
                ReferenceNumber = $"XYZ-{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}"
            };
        }

      
        public static PaymentDetailsDto ToPaymentDetailsDto(this Payment payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            return new PaymentDetailsDto
            {
                PaymentId = payment.PaymentId,
                AdmissionNumber = payment.AdmissionNumber,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                BankReference = payment.BankReference,
               
                PaymentType = payment.PaymentType?.TypeName ?? "N/A",
                PaymentChannel = payment.PaymentChannel?.ChannelName ?? "N/A",
               
                CreatedAt = payment.CreatedAt 
            };
        }

      
        public static PaymentResponseDto ToPaymentResponseDto(this Payment payment, string message)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            return new PaymentResponseDto
            {
                Message = message,
                Payment = payment.ToPaymentDetailsDto()
            };
        }

       
        public static PaymentResponseDto ToPaymentResponseDto(this PaymentDetailsDto paymentDetails, string message)
        {
            if (paymentDetails == null) throw new ArgumentNullException(nameof(paymentDetails));

            return new PaymentResponseDto
            {
                Message = message,
                Payment = paymentDetails
            };
        }
    }
}
