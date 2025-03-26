using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Enums;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetStudentPaymentsAsync(string studentId);
        Task<Payment> ProcessPaymentAsync(Payment payment);
        Task<Payment> GetPaymentAsync(int paymentId);
        Task<bool> CancelPaymentAsync(int paymentId);
        Task<bool> RefundPaymentAsync(int paymentId, string reason);
        Task<decimal> GetStudentBalanceAsync(string studentId);
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
        Task<decimal> GetTotalRevenueAsync();
    }

    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILoggingService _logger;

        public PaymentService(
            ApplicationDbContext context,
            INotificationService notificationService,
            ILoggingService logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<IEnumerable<Payment>> GetStudentPaymentsAsync(string studentId)
        {
            return await _context.Payments
                .Include(p => p.Invoice)
                .Include(p => p.Enrollment)
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment> ProcessPaymentAsync(Payment payment)
        {
            try
            {
                payment.Status = PaymentStatus.Processing;
                payment.PaymentDate = DateTime.UtcNow;
                payment.TransactionId = GenerateTransactionId();

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Process payment logic here
                // This is where you'd integrate with a payment gateway

                payment.Status = PaymentStatus.Completed;
                await _context.SaveChangesAsync();

                var student = await _context.Students.FindAsync(payment.StudentId);
                if (student != null)
                {
                    await _notificationService.SendPaymentConfirmationAsync(
                        student.Email,
                        student.FullName,
                        payment.PaymentId,
                        payment.Amount
                    );
                }

                _logger.LogInformation($"Payment processed successfully: {payment.PaymentId}");

                return payment;
            }
            catch (Exception ex)
            {
                payment.Status = PaymentStatus.Failed;
                await _context.SaveChangesAsync();
                _logger.LogError(ex, $"Error processing payment: {payment.PaymentId}");
                throw new AppException("Payment processing failed", ex);
            }
        }

        public async Task<Payment> GetPaymentAsync(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Invoice)
                .Include(p => p.Enrollment)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                throw new NotFoundException($"Payment {paymentId} not found");
            }

            return payment;
        }

        public async Task<bool> CancelPaymentAsync(int paymentId)
        {
            var payment = await GetPaymentAsync(paymentId);

            if (payment.Status != PaymentStatus.Pending)
            {
                throw new AppException("Only pending payments can be cancelled");
            }

            payment.Status = PaymentStatus.Cancelled;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Payment cancelled: {paymentId}");
            return true;
        }

        public async Task<bool> RefundPaymentAsync(int paymentId, string reason)
        {
            var payment = await GetPaymentAsync(paymentId);

            if (payment.Status != PaymentStatus.Completed)
            {
                throw new AppException("Only completed payments can be refunded");
            }

            // Process refund logic here
            // This is where you'd integrate with a payment gateway

            payment.Status = PaymentStatus.Refunded;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Payment refunded: {paymentId}, Reason: {reason}");
            return true;
        }

        public async Task<decimal> GetStudentBalanceAsync(string studentId)
        {
            var payments = await _context.Payments
                .Where(p => p.StudentId == studentId && p.Status == PaymentStatus.Completed)
                .SumAsync(p => p.Amount);

            var invoices = await _context.Invoices
                .Where(i => i.StudentId == studentId && i.Status != InvoiceStatus.Cancelled)
                .SumAsync(i => i.FinalAmount);

            return invoices - payments;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Include(p => p.Invoice)
                .Include(p => p.Enrollment)
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Include(p => p.Invoice)
                .Include(p => p.Enrollment)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == PaymentStatus.Completed)
                .SumAsync(p => p.Amount);
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }
    }
}