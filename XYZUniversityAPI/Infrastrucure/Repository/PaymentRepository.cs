using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XYZUniversityAPI.Domain.Entities;
using XYZUniversityAPI.Domain.Interfaces;
using XYZUniversityAPI.Infrastructure.Data;

namespace XYZUniversityAPI.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _dbContext;

        public PaymentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Payment>> GetPaymentsByAdmissionNumberAsync(string admissionNumber)
        {
            return await _dbContext.Payments
                .Where(p => p.AdmissionNumber == admissionNumber)
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.PaymentType)
                .Include(p => p.PaymentChannel)
                .ToListAsync();
        }

        // Implementation of missing GetPaymentByReferenceAsync
        public async Task<Payment?> GetPaymentByReferenceAsync(string referenceNumber)
        {
            return await _dbContext.Payments
                .Include(p => p.PaymentType)
                .Include(p => p.PaymentChannel)
                .FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNumber);
        }

        // Implementation of missing UpdatePaymentAsync
        public async Task UpdatePaymentAsync(Payment payment)
        {
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PaymentType>> GetPaymentTypesAsync()
        {
            return await _dbContext.PaymentTypes.ToListAsync();
        }

        public async Task<List<PaymentChannel>> GetPaymentChannelsAsync()
        {
            return await _dbContext.PaymentChannels.ToListAsync();
        }

        // Fixed Typo: Changed Admissison to Admission to match interface
        public async Task<decimal> GetTotalPaidByAdmissionNumberAsync(string admissionNumber)
        {
            return await _dbContext.Payments
                .Where(p =>
                    p.AdmissionNumber == admissionNumber &&
                    p.Status == PaymentStatus.SUCCESS
                )
                .SumAsync(p => p.Amount);
        }
    }
}