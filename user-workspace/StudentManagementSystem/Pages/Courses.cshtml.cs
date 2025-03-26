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
    public class CoursesModel : BasePageModel
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public CoursesModel(
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            ApplicationDbContext context,
            ILogger<CoursesModel> logger)
            : base(context, logger)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        public IList<Course> Courses { get; set; } = new List<Course>();
        public IDictionary<int, int> AvailableSeats { get; set; } = new Dictionary<int, int>();
        public IDictionary<int, bool> EnrollmentStatus { get; set; } = new Dictionary<int, bool>();

        public IList<CourseEnrollment> EnrolledCourses { get; set; } = new List<CourseEnrollment>();

        public async Task<IActionResult> OnGetAsync()
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

          
            Courses = await _courseService.GetAvailableCoursesAsync();

            foreach (var course in Courses)
            {
                var activeEnrollments = await _enrollmentService.GetActiveEnrollmentsCountAsync(course.CourseId);
                AvailableSeats[course.CourseId] = course.MaxSeats - activeEnrollments;
                EnrollmentStatus[course.CourseId] = await _enrollmentService.IsStudentEnrolledAsync(studentId, course.CourseId);
            }

            
            EnrolledCourses = await Context.CourseEnrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.CourseSchedules)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostEnrollAsync(int courseId)
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
                SetErrorMessage($"Failed to enroll in the course: {ex.Message}");
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDropAsync(int courseId)
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                var enrollment = await Context.CourseEnrollments
                    .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

                if (enrollment != null)
                {
                    await _enrollmentService.DropCourseAsync(enrollment.EnrollmentId);
                    SetSuccessMessage("Successfully dropped the course.");
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage($"Failed to drop the course: {ex.Message}");
            }

            return RedirectToPage();
        }
    }
}
