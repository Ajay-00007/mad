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
    public class ScheduleModel : BasePageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;

        public ScheduleModel(
            ApplicationDbContext context,
            ILogger<ScheduleModel> logger,
            IEnrollmentService enrollmentService,
            ICourseService courseService)
            : base(context, logger)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
        }

        public Student Student { get; set; }
        public IList<CourseSchedule> WeeklySchedule { get; set; } = new List<CourseSchedule>();
        public IList<CourseEnrollment> ActiveEnrollments { get; set; } = new List<CourseEnrollment>();
        public IDictionary<int, string> CourseColors { get; set; } = new Dictionary<int, string>();
        public IList<CourseSchedule> TodaysClasses { get; set; } = new List<CourseSchedule>();
        public IList<CourseSchedule> UpcomingClasses { get; set; } = new List<CourseSchedule>();
        public IList<CourseSchedule> ScheduledClasses { get; set; } = new List<CourseSchedule>();
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

                ActiveEnrollments = Student.CourseEnrollments
                    .Where(ce => ce.Status == EnrollmentStatus.Active)
                    .OrderBy(ce => ce.Course.StartDate)
                    .ToList();

                var activeCourseIds = ActiveEnrollments.Select(ce => ce.CourseId).ToList();
                WeeklySchedule = await Context.CourseSchedules
                    .Include(cs => cs.Course)
                    .Where(cs => activeCourseIds.Contains(cs.CourseId))
                    .OrderBy(cs => cs.DayOfWeek)
                    .ThenBy(cs => cs.StartTime)
                    .ToListAsync();

                ScheduledClasses = WeeklySchedule.ToList();

                DayOfWeek today = DateTime.Today.DayOfWeek;
                TodaysClasses = WeeklySchedule
                    .Where(cs => cs.DayOfWeek == today)
                    .OrderBy(cs => cs.StartTime)
                    .ToList();


                var UpcomingClasses = WeeklySchedule
                    .Where(cs => cs.DayOfWeek >= DateTime.Today.DayOfWeek)
                    .OrderBy(cs => cs.DayOfWeek)
                    .ThenBy(cs => cs.StartTime)
                    .ToList();

                // Assign colors to courses for visual distinction
                AssignCourseColors();

                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while loading schedule for student {StudentId}", studentId);
                SetErrorMessage("An error occurred while loading your schedule. Please try again.");
                return RedirectToPage("/Error");
            }
        }

        private void AssignCourseColors()
        {
            var colors = new[]
            {
                "bg-primary",
                "bg-success",
                "bg-info",
                "bg-warning",
                "bg-danger",
                "bg-secondary",
                "bg-dark"
            };

            var colorIndex = 0;
            foreach (var enrollment in ActiveEnrollments)
            {
                if (!CourseColors.ContainsKey(enrollment.CourseId))
                {
                    CourseColors[enrollment.CourseId] = colors[colorIndex % colors.Length];
                    colorIndex++;
                }
            }
        }

        public string GetTimeSlotDisplay(TimeSpan startTime, TimeSpan endTime)
        {
            return $"{startTime:hh\\:mm} - {endTime:hh\\:mm}";
        }

        public string GetCourseColor(int courseId)
        {
            return CourseColors.TryGetValue(courseId, out var color) ? color : "bg-secondary";
        }

        public string GetDayName(DayOfWeek day)
        {
            return day.ToString();
        }
    }
}