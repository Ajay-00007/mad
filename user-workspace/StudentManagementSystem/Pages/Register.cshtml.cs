using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.ViewModels;
using StudentManagementSystem.Services;
using StudentManagementSystem.Infrastructure;

namespace StudentManagementSystem.Pages
{
    public class RegisterModel : BasePageModel
    {
        private readonly IStudentService _studentService;
        private readonly IEmailService _emailService;

        public RegisterModel(
            ApplicationDbContext context,
            ILogger<RegisterModel> logger,
            IStudentService studentService,
            IEmailService emailService)
            : base(context, logger)
        {
            _studentService = studentService;
            _emailService = emailService;
        }

        // Bind RegisterViewModel to the form
        [BindProperty]
        public RegisterViewModel Input { get; set; } = new RegisterViewModel();

        // Store the generated EnrollmentId
        public string? EnrollmentId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                
                
                if (!await _studentService.IsEmailUniqueAsync(Input.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email address is already in use.");
                    return Page();
                }

                
                var student = await _studentService.CreateStudentAsync(Input);

                EnrollmentId = student.StudentId;

                await _emailService.SendWelcomeEmailAsync(Input.Email, EnrollmentId);

                SetSuccessMessage("Registration successful! Please check your email for further instructions.");
                return RedirectToPage("/Account/Login");
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred during registration");
                SetErrorMessage($"{ex}");
                return Page();
            }
        }
    }
}
