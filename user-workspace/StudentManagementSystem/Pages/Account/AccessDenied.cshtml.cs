using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        private readonly ILoggingService _logger;

        public AccessDeniedModel(ILoggingService logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            var userName = User.Identity?.Name ?? "Anonymous";
            var requestedPath = HttpContext.Request.Path;
            var referrerPath = HttpContext.Request.Headers["Referer"].ToString();

            _logger.LogWarning(
                "Access denied for user {UserName} attempting to access {RequestedPath}. Referred from: {ReferrerPath}",
                userName,
                requestedPath,
                referrerPath
            );

            // Additional context logging if user is authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                var userRoles = string.Join(", ", User.Claims
                    .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                    .Select(c => c.Value));

                _logger.LogInformation(
                    "Authenticated user details - Name: {UserName}, Roles: {UserRoles}",
                    userName,
                    userRoles
                );
            }
        }
    }
}