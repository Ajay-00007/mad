using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Pages.Admin.Timetables
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly IAdminService _adminService;
        private readonly IStudentService _studentService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IAdminService adminService,
            IStudentService studentService,
            ILogger<IndexModel> logger)
        {
            _adminService = adminService;
            _studentService = studentService;
            _logger = logger;
        }

        public IEnumerable<Student> Students { get; set; } = new List<Student>();
        public IEnumerable<CourseSchedule> AllSchedules { get; set; } = new List<CourseSchedule>();

        public async Task OnGetAsync()
        {
            Students = await _studentService.GetAllStudentsAsync();
            AllSchedules = await _adminService.GetAllTimetablesAsync();
        }

        public IEnumerable<CourseSchedule> GetSchedulesForTimeSlot(string day, string time)
        {
            if (Enum.TryParse<DayOfWeek>(day, out var dayOfWeek))
            {
                return AllSchedules.Where(s => 
                    s.DayOfWeek == dayOfWeek && 
                    s.StartTime.ToString("HH:mm") == time);
            }
            return Enumerable.Empty<CourseSchedule>();
        }

        public async Task<IActionResult> OnGetStudentScheduleAsync(string studentId)
        {
            try
            {
                var schedule = await _adminService.GetStudentTimetableAsync(studentId);
                var student = await _studentService.GetStudentByIdAsync(studentId);

                if (schedule == null || student == null)
                {
                    return NotFound();
                }

                // Convert schedule to a format suitable for the frontend
                var scheduleData = new
                {
                    StudentName = student.FullName,
                    StudentId = student.StudentId,
                    Schedules = schedule.Course.CourseSchedules.Select(cs => new
                    {
                        cs.DayOfWeek,
                        cs.StartTime,
                        cs.EndTime,
                        cs.Room,
                        CourseName = schedule.Course.CourseName
                    })
                };

                return new JsonResult(scheduleData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching schedule for student {studentId}");
                return StatusCode(500, "Failed to fetch student schedule");
            }
        }

        public class ScheduleViewModel
        {
            public string DayOfWeek { get; set; } = string.Empty;
            public string StartTime { get; set; } = string.Empty;
            public string EndTime { get; set; } = string.Empty;
            public string Room { get; set; } = string.Empty;
            public string CourseName { get; set; } = string.Empty;
        }
    }
}