using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ILoggingService _logger;

        public LogoutModel(ILoggingService logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                var userName = User.Identity?.Name;

                // Clear the existing external cookie
                await HttpContext.SignOutAsync("CookieAuth");
                
                // Clear session
                HttpContext.Session.Clear();

                if (!string.IsNullOrEmpty(userName))
                {
                    _logger.LogInformation($"User {userName} logged out successfully");
                }

                TempData["SuccessMessage"] = "You have been successfully logged out.";
                return RedirectToPage("/Account/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout process");
                TempData["ErrorMessage"] = "An error occurred during logout. Please try again.";
                return Page();
            }
        }
    }
}