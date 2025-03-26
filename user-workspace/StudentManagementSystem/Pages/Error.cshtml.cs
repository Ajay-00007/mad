using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        private readonly ILoggingService _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorModel(ILoggingService logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public bool IsProduction => _environment.IsProduction();
        public string? ErrorMessage { get; set; }
        public Exception? Exception { get; set; }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            var exceptionHandler = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            
            if (exceptionHandler != null)
            {
                Exception = exceptionHandler.Error;
                ErrorMessage = Exception.Message;

                // Log the error
                _logger.LogError(Exception, 
                    "Error occurred. RequestId: {RequestId}, Path: {Path}, Message: {Message}",
                    RequestId,
                    exceptionHandler.Path,
                    ErrorMessage);

                // Set a user-friendly message in production
                if (IsProduction)
                {
                    ErrorMessage = "An unexpected error occurred. Please try again later.";
                }
            }
            else
            {
                ErrorMessage = "An error occurred while processing your request.";
                _logger.LogWarning("Error page accessed directly. RequestId: {RequestId}", RequestId);
            }

            // Log additional request details
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var referrer = HttpContext.Request.Headers["Referer"].ToString();
            
            _logger.LogInformation(
                "Error page accessed. RequestId: {RequestId}, UserAgent: {UserAgent}, Referrer: {Referrer}",
                RequestId,
                userAgent,
                referrer);
        }

        public IActionResult OnPost()
        {
            // Handle POST requests to the error page (e.g., from error feedback form)
            return RedirectToPage("/Index");
        }
    }
}
