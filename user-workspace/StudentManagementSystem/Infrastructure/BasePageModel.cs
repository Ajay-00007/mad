using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Infrastructure
{
    public abstract class BasePageModel : PageModel
    {
        protected readonly ApplicationDbContext Context;
        protected readonly ILogger Logger;

        protected BasePageModel(ApplicationDbContext context, ILogger logger)
        {
            Context = context;
            Logger = logger;
        }

        // Add a constructor that accepts ILogger<T>
        protected BasePageModel(ApplicationDbContext context, ILogger<BasePageModel> logger)
            : this(context, (ILogger)logger)
        {
        }

        public Student? CurrentUser
        {
            get => HttpContext.Items["CurrentUser"] as Student;
        }

        protected virtual bool RequiresAuthentication => true;

        public bool IsAuthenticated => CurrentUser != null;

        protected string CurrentStudentId => CurrentUser?.StudentId ?? string.Empty;

        protected IActionResult RedirectToLoginPage()
        {
            return RedirectToPage("/Index");
        }

        protected void SetSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }

        protected void SetErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }

        protected void LogInformation(string message)
        {
            Logger.LogInformation(message);
        }

        protected void LogWarning(string message)
        {
            Logger.LogWarning(message);
        }

        protected void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                Logger.LogError(ex, message);
            }
            else
            {
                Logger.LogError(message);
            }
        }
    }
}