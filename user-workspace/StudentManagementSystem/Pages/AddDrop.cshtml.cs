using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Models.Enums;
using StudentManagementSystem.Extensions;

namespace StudentManagementSystem.Pages
{
    public class AddDropModel : BasePageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;

        public AddDropModel(
            ApplicationDbContext context,
            ILogger<AddDropModel> logger,
            IEnrollmentService enrollmentService,
            ICourseService courseService)
            : base(context, logger)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
        }

        public IList<Course> AvailableCourses { get; set; } = new List<Course>();
        public IList<CourseEnrollment> CurrentEnrollments { get; set; } = new List<CourseEnrollment>();

        public async Task<IActionResult> OnGetAsync()
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                AvailableCourses = await _courseService.GetAvailableCoursesAsync();
                CurrentEnrollments = (await _enrollmentService.GetStudentEnrollmentsAsync(studentId))
                    .Where(e => e.Status == EnrollmentStatus.Active)
                    .ToList();

                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while loading add/drop page for student {StudentId}", studentId);
                SetErrorMessage("An error occurred while loading the page. Please try again.");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAddAsync(int courseId)
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                await _enrollmentService.EnrollStudentAsync(studentId, courseId);
                SetSuccessMessage("Successfully enrolled in the course.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error enrolling student {StudentId} in course {CourseId}", studentId, courseId);
                SetErrorMessage($"Failed to enroll in the course: {ex.Message}");
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDropAsync(int enrollmentId)
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                await _enrollmentService.UpdateEnrollmentStatusAsync(enrollmentId, EnrollmentStatus.Dropped);
                SetSuccessMessage("Successfully dropped the course.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error dropping enrollment {EnrollmentId} for student {StudentId}", enrollmentId, studentId);
                SetErrorMessage($"Failed to drop the course: {ex.Message}");
            }

            return RedirectToPage();
        }
    }
}