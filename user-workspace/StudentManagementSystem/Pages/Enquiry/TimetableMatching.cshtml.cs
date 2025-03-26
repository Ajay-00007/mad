using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Extensions;

namespace StudentManagementSystem.Pages.Enquiry
{
    public class TimetableMatchingModel : BasePageModel
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public TimetableMatchingModel(
            ApplicationDbContext context,
            ILogger<TimetableMatchingModel> logger,
            ICourseService courseService,
            IEnrollmentService enrollmentService)
            : base(context, logger)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        public Student Student { get; set; }
        public IList<CourseSchedule> CurrentSchedule { get; set; } = new List<CourseSchedule>();
        public IList<Course> AvailableCourses { get; set; } = new List<Course>();
        public IDictionary<int, bool> ConflictingCourses { get; set; } = new Dictionary<int, bool>();

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
                    .Include(s => s.CourseEnrollments)
                        .ThenInclude(ce => ce.Course)
                            .ThenInclude(c => c.CourseSchedules)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (Student == null)
                {
                    return NotFound();
                }

                // Get current schedule
                CurrentSchedule = Student.CourseEnrollments
                    .Where(ce => ce.Status == Models.Enums.EnrollmentStatus.Active)
                    .SelectMany(ce => ce.Course.CourseSchedules)
                    .OrderBy(cs => cs.DayOfWeek)
                    .ThenBy(cs => cs.StartTime)
                    .ToList();

                // Get available courses
                AvailableCourses = await _courseService.GetAvailableCoursesAsync();

                // Check for schedule conflicts
                foreach (var course in AvailableCourses)
                {
                    ConflictingCourses[course.CourseId] = HasScheduleConflict(course.CourseSchedules);
                }

                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while loading timetable matching for student {StudentId}", studentId);
                SetErrorMessage("An error occurred while loading the timetable matching. Please try again.");
                return RedirectToPage("/Error");
            }
        }

        private bool HasScheduleConflict(ICollection<CourseSchedule> newSchedules)
        {
            foreach (var currentSlot in CurrentSchedule)
            {
                foreach (var newSlot in newSchedules)
                {
                    if (currentSlot.DayOfWeek == newSlot.DayOfWeek)
                    {
                        // Check for time overlap
                        if ((newSlot.StartTime >= currentSlot.StartTime && newSlot.StartTime < currentSlot.EndTime) ||
                            (newSlot.EndTime > currentSlot.StartTime && newSlot.EndTime <= currentSlot.EndTime) ||
                            (newSlot.StartTime <= currentSlot.StartTime && newSlot.EndTime >= currentSlot.EndTime))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
