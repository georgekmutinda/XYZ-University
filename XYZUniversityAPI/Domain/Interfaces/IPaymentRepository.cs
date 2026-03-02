using System.Collections.Generic;
using System.Threading.Tasks;
using XYZUniversityAPI.Domain.Entities;

namespace XYZUniversityAPI.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        // Create payment intent
        Task AddPaymentAsync(Payment payment);

        // Query payments for a student
        Task<List<Payment>> GetPaymentsByAdmissionNumberAsync(string admissionNumber);

        // Lookup payment by internal reference (for bank callbacks)
        Task<Payment?> GetPaymentByReferenceAsync(string referenceNumber);

        // Persist bank confirmation updates
        Task UpdatePaymentAsync(Payment payment);

        // Reference data
        Task<List<PaymentType>> GetPaymentTypesAsync();
        Task<List<PaymentChannel>> GetPaymentChannelsAsync();

        // Financial aggregates (confirmed only)
        Task<decimal> GetTotalPaidByAdmissionNumberAsync(string admissionNumber);
    }
}
