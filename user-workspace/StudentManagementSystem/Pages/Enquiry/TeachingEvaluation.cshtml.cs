using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Infrastructure;

namespace StudentManagementSystem.Pages.Enquiry
{
    public class TeachingEvaluationModel : BasePageModel
    {
        public Student? Student { get; set; }
        public List<TeachingEvaluation> Evaluations { get; set; } = new();
        
        // Add properties referenced in the view
        public TeachingEvaluation? Evaluation { get; set; }
        public List<CourseEnrollment> EnrolledCourses { get; set; } = new();
        public List<TeachingEvaluation> PreviousEvaluations { get; set; } = new();

        [BindProperty]
        public TeachingEvaluation NewEvaluation { get; set; } = new();

        public TeachingEvaluationModel(ApplicationDbContext context, ILogger<TeachingEvaluationModel> logger)
            : base(context, logger)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(CurrentStudentId))
            {
                return RedirectToLoginPage();
            }

            Student = await Context.Students
                .Include(s => s.CourseEnrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentId == CurrentStudentId);

            if (Student == null)
            {
                return NotFound();
            }

            // Get enrolled courses
            EnrolledCourses = await Context.CourseEnrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == CurrentStudentId)
                .ToListAsync();

            // Get previous evaluations
            PreviousEvaluations = await Context.TeachingEvaluations
                .Where(e => e.StudentId == CurrentStudentId)
                .Include(e => e.Course)
                .OrderByDescending(e => e.SubmittedAt)
                .ToListAsync();

            // Get current evaluation if exists
            Evaluation = await Context.TeachingEvaluations
                .Include(e => e.Course)
                .Where(e => e.StudentId == CurrentStudentId)
                .OrderByDescending(e => e.SubmittedAt)
                .FirstOrDefaultAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(TeachingEvaluation evaluation)
        {
            if (string.IsNullOrEmpty(CurrentStudentId))
            {
                return RedirectToLoginPage();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            evaluation.StudentId = CurrentStudentId;
            evaluation.SubmittedAt = DateTime.UtcNow;

            Context.TeachingEvaluations.Add(evaluation);
            await Context.SaveChangesAsync();

            SetSuccessMessage("Teaching evaluation submitted successfully.");
            return RedirectToPage();
        }
    }
}
