using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Infrastructure;

namespace StudentManagementSystem.Pages.Payments
{
    public class ReceiptModel : BasePageModel
    {
        public Payment? Payment { get; set; }
        public Invoice? RelatedInvoice { get; set; }

        public ReceiptModel(ApplicationDbContext context, ILogger<ReceiptModel> logger)
            : base(context, logger)
        {
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (string.IsNullOrEmpty(CurrentStudentId))
            {
                return RedirectToLoginPage();
            }

            // Get payment with student details
            Payment = await Context.Payments
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.PaymentId == id && p.EnrollmentId == CurrentStudentId);

            if (Payment == null)
            {
                return NotFound();
            }

            // Get related invoice if exists
            RelatedInvoice = await Context.Invoices
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Payments.Any(p => p.PaymentId == id));

            return Page();
        }
    }
}