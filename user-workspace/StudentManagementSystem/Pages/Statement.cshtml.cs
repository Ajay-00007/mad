using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Enums;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Services;
using StudentManagementSystem.Extensions;

namespace StudentManagementSystem.Pages
{
    public class StatementModel : BasePageModel
    {
        private readonly IPaymentService _paymentService;

        public StatementModel(
            ApplicationDbContext context,
            ILogger<StatementModel> logger,
            IPaymentService paymentService)
            : base(context, logger)
        {
            _paymentService = paymentService;
        }

        public Student Student { get; set; }
        public IList<Payment> Payments { get; set; } = new List<Payment>();
        public IList<Invoice> Invoices { get; set; } = new List<Invoice>();
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public decimal Balance { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                Student = await Context.Students
                    .Include(s => s.Payments)
                    .Include(s => s.Invoices)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (Student == null)
                {
                    return NotFound();
                }

                // Get completed payments
                Payments = await Context.Payments
                    .Include(p => p.Invoice)
                    .Where(p => p.StudentId == studentId && p.Status == PaymentStatus.Completed)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();

                // Get active invoices
                Invoices = await Context.Invoices
                    .Include(i => i.Items)
                    .Where(i => i.StudentId == studentId && i.Status != InvoiceStatus.Cancelled)
                    .OrderByDescending(i => i.DueDate)
                    .ToListAsync();

                // Calculate totals
                TotalPaid = Payments.Sum(p => p.Amount);
                TotalDue = Invoices.Sum(i => i.FinalAmount);
                Balance = await _paymentService.GetStudentBalanceAsync(studentId);

                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while loading statement for student {StudentId}", studentId);
                SetErrorMessage("An error occurred while loading your statement. Please try again.");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnGetDownloadAsync(int invoiceId)
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                var invoice = await Context.Invoices
                    .Include(i => i.Items)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.StudentId == studentId);

                if (invoice == null)
                {
                    return NotFound();
                }

                // Generate PDF statement
                var pdfContent = await GenerateInvoicePdfAsync(invoice);
                var fileName = $"Invoice_{invoice.InvoiceId}_{DateTime.Now:yyyyMMdd}.pdf";

                return File(pdfContent, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while downloading invoice {InvoiceId} for student {StudentId}", invoiceId, studentId);
                SetErrorMessage("An error occurred while downloading the invoice. Please try again.");
                return RedirectToPage();
            }
        }

        private async Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice)
        {
            // Implementation of PDF generation
            // This would typically use a PDF library like iTextSharp or PDFsharp
            throw new NotImplementedException("PDF generation not implemented");
        }
    }
}
