using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILoggingService _logger;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;

        public IndexModel(
            ILoggingService logger,
            ICourseService courseService,
            IStudentService studentService)
        {
            _logger = logger;
            _courseService = courseService;
            _studentService = studentService;
        }

        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int ActiveEnrollments { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var courses = await _courseService.GetAllCoursesAsync();
                TotalCourses = courses.Count();

                var students = await _studentService.GetAllStudentsAsync();
                TotalStudents = students.Count();

                ActiveEnrollments = courses.Sum(c => c.CourseEnrollments.Count(e => 
                    e.Status == Models.Enums.EnrollmentStatus.Active));

                _logger.LogInformation("Index page loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading index page statistics");
                TotalCourses = 0;
                TotalStudents = 0;
                ActiveEnrollments = 0;
            }
        }
    }
}
