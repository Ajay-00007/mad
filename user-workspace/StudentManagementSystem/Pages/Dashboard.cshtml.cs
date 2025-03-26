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
    public class DashboardModel : BasePageModel
    {
        private readonly IStudentService _studentService;
        private readonly IPaymentService _paymentService;
        private readonly IEnrollmentService _enrollmentService;

        public DashboardModel(
            ApplicationDbContext context,
            ILogger<DashboardModel> logger,
            IStudentService studentService,
            IPaymentService paymentService,
            IEnrollmentService enrollmentService)
            : base(context, logger)
        {
            _studentService = studentService;
            _paymentService = paymentService;
            _enrollmentService = enrollmentService;
        }

        public Student Student { get; set; }
        public IList<CourseEnrollment> ActiveEnrollments { get; set; } = new List<CourseEnrollment>();
        public IList<Payment> RecentPayments { get; set; } = new List<Payment>();
        public IList<Invoice> PendingInvoices { get; set; } = new List<Invoice>();
        public decimal CurrentBalance { get; set; }
        public int ActiveCoursesCount { get; set; }
        public int CompletedCoursesCount { get; set; }
        public int EnrolledCoursesCount => ActiveEnrollments.Count;
        public string StudentName { get; set; } = string.Empty;
        public IList<CourseEnrollment> RecentEnrollments { get; set; } = new List<CourseEnrollment>();
        public int TodaysClassesCount { get; set; }
        public decimal TotalPayments { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                Student = await _studentService.GetStudentByIdAsync(studentId);

                // Get active enrollments
                var enrollments = await _enrollmentService.GetStudentEnrollmentsAsync(studentId);
                ActiveEnrollments = enrollments
                    .Where(e => e.Status == EnrollmentStatus.Active)
                    .OrderBy(e => e.Course.StartDate)
                    .ToList();

                // Get recent payments
                RecentPayments = (await _paymentService.GetStudentPaymentsAsync(studentId))
                    .Where(p => p.Status == PaymentStatus.Completed)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .ToList();

                // Get pending invoices
                PendingInvoices = await Context.Invoices
                    .Where(i => i.StudentId == studentId && 
                               i.Status == InvoiceStatus.Pending)
                    .OrderBy(i => i.DueDate)
                    .ToListAsync();

                // Get current balance
                CurrentBalance = await _paymentService.GetStudentBalanceAsync(studentId);

                // Get course statistics
                ActiveCoursesCount = enrollments.Count(e => e.Status == EnrollmentStatus.Active);
                CompletedCoursesCount = enrollments.Count(e => e.Status == EnrollmentStatus.Completed);

                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while loading dashboard for student {StudentId}", studentId);
                SetErrorMessage("An error occurred while loading your dashboard. Please try again.");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostPayInvoiceAsync(int invoiceId)
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                var invoice = await Context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.StudentId == studentId);

                if (invoice == null)
                {
                    return NotFound();
                }

                // Create payment
                var payment = new Payment
                {
                    StudentId = studentId,
                    InvoiceId = invoiceId,
                    Amount = invoice.FinalAmount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = PaymentMethod.OnlinePayment,
                    Status = PaymentStatus.Pending
                };

                await _paymentService.ProcessPaymentAsync(payment);
                SetSuccessMessage("Payment processed successfully.");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing payment for invoice {InvoiceId}", invoiceId);
                SetErrorMessage("An error occurred while processing your payment. Please try again.");
                return RedirectToPage();
            }
        }
    }
}