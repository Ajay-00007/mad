using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Pages.Admin.Courses
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly IAdminService _adminService;
        private readonly ICourseService _courseService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IAdminService adminService,
            ICourseService courseService,
            ILogger<IndexModel> logger)
        {
            _adminService = adminService;
            _courseService = courseService;
            _logger = logger;
        }

        public IEnumerable<Course> Courses { get; set; } = new List<Course>();

        public async Task OnGetAsync()
        {
            Courses = await _adminService.GetAllCoursesAsync();
        }

        public async Task<IActionResult> OnPostUpdateFeeAsync(int courseId, decimal fee)
        {
            try
            {
                await _adminService.UpdateCourseFeeAsync(courseId, fee);
                _logger.LogInformation($"Updated course {courseId} fee to ${fee}");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating course {courseId} fee");
                TempData["ErrorMessage"] = "Failed to update course fee. Please try again.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostBulkUpdateFeesAsync(decimal bulkFee)
        {
            try
            {
                var courses = await _adminService.GetAllCoursesAsync();
                foreach (var course in courses)
                {
                    await _adminService.UpdateCourseFeeAsync(course.CourseId, bulkFee);
                }
                
                _logger.LogInformation($"Bulk updated all course fees to ${bulkFee}");
                TempData["SuccessMessage"] = "Successfully updated all course fees.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk fee update");
                TempData["ErrorMessage"] = "Failed to update course fees. Please try again.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnGetDetailsAsync(int id)
        {
            var courses = await _adminService.GetAllCoursesAsync();
            var course = courses.FirstOrDefault(c => c.CourseId == id);
            
            if (course == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                course.CourseId,
                course.CourseName,
                course.Description,
                course.Credits,
                course.MaxSeats,
                course.Fee,
                EnrolledStudents = course.CourseEnrollments.Count,
                Schedules = course.CourseSchedules.Select(cs => new
                {
                    cs.DayOfWeek,
                    cs.StartTime,
                    cs.EndTime,
                    cs.Room
                })
            });
        }
    }
}