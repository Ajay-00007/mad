using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Models.Enums;

namespace StudentManagementSystem.Pages
{
    public class PaymentsModel : BasePageModel
    {
        private readonly IPaymentService _paymentService;
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public decimal TotalPaid { get; set; }
        public decimal PendingAmount { get; set; }

        public PaymentsModel(
            ApplicationDbContext context,
            ILogger<PaymentsModel> logger,
            IPaymentService paymentService) : base(context, logger)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get student's payments
                var payments = await _paymentService.GetStudentPaymentsAsync(CurrentStudentId);
                Payments = payments.ToList();

                // Calculate totals
                TotalPaid = Payments
                    .Where(p => p.Status == PaymentStatus.Completed)
                    .Sum(p => p.Amount);

                PendingAmount = Payments
                    .Where(p => p.Status == PaymentStatus.Pending)
                    .Sum(p => p.Amount);

                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while loading payments for student {StudentId}", CurrentStudentId);
                SetErrorMessage("An error occurred while loading the payments. Please try again.");
                return RedirectToPage("/Error");
            }
        }
    }
}