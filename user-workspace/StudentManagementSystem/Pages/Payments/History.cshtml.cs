using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Models.Enums;

namespace StudentManagementSystem.Pages.Payments
{
    public class HistoryModel : BasePageModel
    {
        public IList<Payment> Payments { get; set; } = new List<Payment>();
        public IList<Invoice> Invoices { get; set; } = new List<Invoice>();

        // Add properties for filtering and summary
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CompletedAmount { get; set; }
        public decimal PendingAmount { get; set; }

        public HistoryModel(ApplicationDbContext context, ILogger<HistoryModel> logger)
            : base(context, logger)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(CurrentStudentId))
            {
                return RedirectToLoginPage();
            }

            // Get payments with student details
            Payments = await Context.Payments
                .Include(p => p.Student)
                .Where(p => p.StudentId == CurrentStudentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            // Get invoices with student details
            Invoices = await Context.Invoices
                .Include(i => i.Student)
                .Include(i => i.Payments)
                .Where(i => i.StudentId == CurrentStudentId)
                .OrderByDescending(i => i.DueDate)
                .ToListAsync();

            // Calculate summary amounts
            TotalAmount = Payments.Sum(p => p.Amount);
            CompletedAmount = Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
            PendingAmount = Payments.Where(p => p.Status == PaymentStatus.Pending).Sum(p => p.Amount);

            return Page();
        }
    }
}