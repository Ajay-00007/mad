using Microsoft.Extensions.Options;
using StudentManagementSystem.Configuration;
using StudentManagementSystem.Services;
using System.Security.Claims;

namespace StudentManagementSystem.Middleware
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly ILoggingService _logger;

        public SessionMiddleware(
            RequestDelegate next,
            IOptions<AppSettings> appSettings,
            ILoggingService logger)
        {
            _next = next;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (RequiresSessionValidation(context))
                {
                    if (!await ValidateSessionAsync(context))
                    {
                        await HandleInvalidSessionAsync(context);
                        return;
                    }

                    await UpdateSessionActivityAsync(context);
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in session middleware");
                throw;
            }
        }

        private bool RequiresSessionValidation(HttpContext context)
        {
            // Skip session validation for certain paths
            var skipPaths = new[]
            {
                "/account/login",
                "/account/logout",
                "/account/register",
                "/api/",
                "/health",
                "/favicon.ico",
                "/css/",
                "/js/",
                "/images/"
            };

            var path = context.Request.Path.Value?.ToLowerInvariant();
            return !skipPaths.Any(p => path?.StartsWith(p) ?? false);
        }

        private async Task<bool> ValidateSessionAsync(HttpContext context)
        {
            var sessionId = context.Session.GetString("SessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            var lastActivity = context.Session.GetString("LastActivity");
            if (string.IsNullOrEmpty(lastActivity) || 
                !DateTime.TryParse(lastActivity, out DateTime lastActivityTime))
            {
                return false;
            }

            // Check session timeout
            var timeoutMinutes = _appSettings.SessionTimeoutMinutes;
            if (DateTime.UtcNow.Subtract(lastActivityTime).TotalMinutes > timeoutMinutes)
            {
                return false;
            }

            // Validate user claims if needed
            var userType = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userType))
            {
                return false;
            }

            return true;
        }

        private async Task HandleInvalidSessionAsync(HttpContext context)
        {
            // Clear existing session
            context.Session.Clear();

            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Handle AJAX requests
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Session expired" });
            }
            else
            {
                // Redirect to login page for regular requests
                context.Response.Redirect("/Account/Login?returnUrl=" + 
                    Uri.EscapeDataString(context.Request.Path + context.Request.QueryString));
            }
        }

        private async Task UpdateSessionActivityAsync(HttpContext context)
        {
            context.Session.SetString("LastActivity", DateTime.UtcNow.ToString("O"));

            // Update user's last activity in database if needed
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var userType = context.User.FindFirst(ClaimTypes.Role)?.Value;
                await UpdateUserLastActivityAsync(userId, userType);
            }
        }

        private async Task UpdateUserLastActivityAsync(string userId, string? userType)
        {
            try
            {
                // This would typically update the LastLoginDate in your user tables
                // Implementation would depend on your actual data access layer
                _logger.LogInformation($"Updated last activity for user {userId} ({userType})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating last activity for user {userId}");
            }
        }
    }

    // Extension method to add the middleware to the application pipeline
    public static class SessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionMiddleware>();
        }
    }
}
