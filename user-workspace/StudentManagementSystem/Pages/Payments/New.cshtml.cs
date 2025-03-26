using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Enums;
using StudentManagementSystem.Services;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Extensions;

namespace StudentManagementSystem.Pages.Payments
{
    public class NewModel : BasePageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;

        public NewModel(
            ApplicationDbContext context,
            ILogger<NewModel> logger,
            IPaymentService paymentService,
            INotificationService notificationService)
            : base(context, logger)
        {
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        [BindProperty]
        public Payment Payment { get; set; } = new Payment();

        public Invoice Invoice { get; set; }
        public IList<PaymentMethod> AvailablePaymentMethods { get; set; }

        [BindProperty]
        public string CardNumber { get; set; }

        [BindProperty]
        public string ExpiryDate { get; set; }

        [BindProperty]
        public string CVV { get; set; }

        [BindProperty]
        public string CardHolderName { get; set; }

        public async Task<IActionResult> OnGetAsync(int invoiceId)
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                Invoice = await Context.Invoices
                    .Include(i => i.Items)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.StudentId == studentId);

                if (Invoice == null)
                {
                    return NotFound();
                }

                if (Invoice.Status != InvoiceStatus.Pending)
                {
                    SetErrorMessage("This invoice cannot be paid.");
                    return RedirectToPage("./Index");
                }

                // Initialize payment
                Payment = new Payment
                {
                    StudentId = studentId,
                    InvoiceId = invoiceId,
                    Amount = Invoice.FinalAmount,
                    PaymentDate = DateTime.UtcNow
                };

                // Get available payment methods
                AvailablePaymentMethods = new List<PaymentMethod>
                {
                    PaymentMethod.CreditCard,
                    PaymentMethod.BankTransfer,
                    PaymentMethod.OnlinePayment
                };

                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while loading payment page for invoice {InvoiceId}", invoiceId);
                SetErrorMessage("An error occurred while loading the payment page. Please try again.");
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Verify invoice exists and belongs to student
                var invoice = await Context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceId == Payment.InvoiceId && i.StudentId == studentId);

                if (invoice == null)
                {
                    return NotFound();
                }

                // Process payment
                Payment.Status = PaymentStatus.Processing;
                var processedPayment = await _paymentService.ProcessPaymentAsync(Payment);

                if (processedPayment.Status == PaymentStatus.Completed)
                {
                    // Update invoice status
                    invoice.Status = InvoiceStatus.Paid;
                    invoice.PaidDate = DateTime.UtcNow;
                    await Context.SaveChangesAsync();

                    // Send confirmation
                    await _notificationService.SendPaymentConfirmationAsync(
                        studentId,
                        invoice.Student.FullName,
                        processedPayment.PaymentId,
                        processedPayment.Amount
                    );

                    SetSuccessMessage("Payment processed successfully.");
                    return RedirectToPage("./Receipt", new { id = processedPayment.PaymentId });
                }
                else if (processedPayment.Status == PaymentStatus.Failed)
                {
                    SetErrorMessage("Payment processing failed. Please try again.");
                    return Page();
                }
                else
                {
                    SetErrorMessage("Payment is being processed. Please check the status later.");
                    return RedirectToPage("./History");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while processing payment for invoice {InvoiceId}", Payment.InvoiceId);
                SetErrorMessage("An error occurred while processing your payment. Please try again.");
                return Page();
            }
        }
    }
}