using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILoggingService _logger;

        public PrivacyModel(ILoggingService logger)
        {
            _logger = logger;
        }

        public DateTime LastUpdated { get; private set; } = new DateTime(2024, 3, 22);

        public void OnGet()
        {
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            _logger.LogInformation(
                "Privacy policy accessed. User Agent: {UserAgent}",
                userAgent
            );
        }
    }
}
