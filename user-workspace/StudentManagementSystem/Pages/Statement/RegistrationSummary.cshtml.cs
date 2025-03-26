using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Models.Enums;

namespace StudentManagementSystem.Pages.Statement
{
    public class RegistrationSummaryModel : BasePageModel
    {
        public Student? Student { get; set; }
        public List<CourseEnrollment> Enrollments { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PendingAmount { get; set; }

        // Add properties referenced in the view
        public RegistrationSummary? CurrentRegistration { get; set; }
        public List<RegistrationSummary> PreviousRegistrations { get; set; } = new();

        public RegistrationSummaryModel(ApplicationDbContext context, ILogger<RegistrationSummaryModel> logger)
            : base(context, logger)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(CurrentStudentId))
            {
                return RedirectToLoginPage();
            }

            // Get student details with enrollments
            Student = await Context.Students
                .Include(s => s.CourseEnrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentId == CurrentStudentId);

            if (Student == null)
            {
                return NotFound();
            }

            // Get payments
            Payments = await Context.Payments
                .Where(p => p.StudentId == CurrentStudentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            // Get current registration
            CurrentRegistration = await Context.RegistrationSummaries
                .Include(r => r.Enrollments)
                .ThenInclude(e => e.Course)
                .Where(r => r.StudentId == CurrentStudentId)
                .OrderByDescending(r => r.RegistrationDate)
                .FirstOrDefaultAsync();

            // Get previous registrations
            PreviousRegistrations = await Context.RegistrationSummaries
                .Include(r => r.Enrollments)
                .ThenInclude(e => e.Course)
                .Where(r => r.StudentId == CurrentStudentId)
                .OrderByDescending(r => r.RegistrationDate)
                .Skip(1) // Skip current registration
                .ToListAsync();

            // Calculate totals
            TotalAmount = Student.CourseEnrollments.Sum(e => e.Course?.Fee ?? 0);
            PaidAmount = Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
            PendingAmount = TotalAmount - PaidAmount;

            return Page();
        }
    }
}
