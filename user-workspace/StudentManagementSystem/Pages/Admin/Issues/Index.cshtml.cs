using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Pages.Admin.Issues
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IAdminService adminService, ILogger<IndexModel> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public IEnumerable<StudentIssue> Issues { get; set; } = new List<StudentIssue>();

        public async Task OnGetAsync(string? status = null)
        {
            Issues = await _adminService.GetStudentIssuesAsync(status);
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int issueId, string status, string? resolution)
        {
            try
            {
                await _adminService.UpdateIssueStatusAsync(issueId, status, resolution);
                _logger.LogInformation($"Updated issue {issueId} status to {status}");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating issue {issueId} status");
                TempData["ErrorMessage"] = "Failed to update issue status. Please try again.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnGetDetailsAsync(int id)
        {
            var issues = await _adminService.GetStudentIssuesAsync();
            var issue = issues.FirstOrDefault(i => i.IssueId == id);
            
            if (issue == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                issue.IssueId,
                issue.Title,
                issue.Description,
                issue.Status,
                issue.Resolution,
                StudentName = issue.Student.FullName,
                CreatedAt = issue.CreatedAt.ToString("MMM dd, yyyy HH:mm"),
                UpdatedAt = issue.UpdatedAt.ToString("MMM dd, yyyy HH:mm"),
                ResolvedAt = issue.ResolvedAt?.ToString("MMM dd, yyyy HH:mm")
            });
        }
    }
}