using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class DashboardModel : PageModel
    {
        private readonly IAdminService _adminService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IPaymentService _paymentService;

        public DashboardModel(
            IAdminService adminService,
            IStudentService studentService,
            ICourseService courseService,
            IPaymentService paymentService)
        {
            _adminService = adminService;
            _studentService = studentService;
            _courseService = courseService;
            _paymentService = paymentService;
        }

        public int TotalStudents { get; set; }
        public int ActiveCourses { get; set; }
        public int OpenIssues { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<StudentIssue> RecentIssues { get; set; } = new List<StudentIssue>();
        public IEnumerable<Course> RecentCourses { get; set; } = new List<Course>();

        public async Task OnGetAsync()
        {
            // Get dashboard statistics
            var issues = await _adminService.GetStudentIssuesAsync();
            var courses = await _adminService.GetAllCoursesAsync();

            TotalStudents = await _studentService.GetTotalStudentsCountAsync();
            ActiveCourses = courses.Count();
            OpenIssues = issues.Count(i => i.Status == "Open");
            TotalRevenue = await _paymentService.GetTotalRevenueAsync();

            // Get recent issues (last 5)
            RecentIssues = issues
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .ToList();

            // Get recent courses (last 5)
            RecentCourses = courses
                .OrderByDescending(c => c.CourseEnrollments.Count)
                .Take(5)
                .ToList();
        }
    }
}