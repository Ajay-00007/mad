using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using StudentManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace StudentManagementSystem.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IStudentService _studentService;
        private readonly IAdminService _adminService;
        private readonly ILoggingService _logger;

        public LoginModel(
            IStudentService studentService,
            IAdminService adminService,
            ILoggingService logger)
        {
            _studentService = studentService;
            _adminService = adminService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Date of Birth is required")]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime DateOfBirth { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            await HttpContext.SignOutAsync();

            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                try
                {
                    // First try admin login
                    var admin = await _adminService.GetAdminByEmailAsync(Input.Email);
                    if (admin != null && await _adminService.ValidateCredentialsAsync(Input.Email, Input.DateOfBirth.ToString("yyyy-MM-dd")))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, admin.Email),
                            new Claim(ClaimTypes.Role, "Admin"),
                            new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                            new Claim(ClaimTypes.GivenName, admin.FirstName),
                            new Claim(ClaimTypes.Surname, admin.LastName)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = Input.RememberMe
                        };

                        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                        _logger.LogInformation($"Admin {admin.Email} logged in successfully");
                        return RedirectToPage("/Admin/Dashboard");
                    }

                    // Then try student login
                    var student = await _studentService.GetStudentByEmailAsync(Input.Email);
                    if (student != null && student.DateOfBirth.Date == Input.DateOfBirth.Date)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, student.Email),
                            new Claim(ClaimTypes.Role, "Student"),
                            new Claim(ClaimTypes.NameIdentifier, student.StudentId),
                            new Claim(ClaimTypes.GivenName, student.FirstName),
                            new Claim(ClaimTypes.Surname, student.LastName)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = Input.RememberMe
                        };

                        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);
                        _logger.LogInformation($"Student {student.Email} logged in successfully");
                        return RedirectToPage("/Dashboard");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    _logger.LogWarning($"Failed login attempt for email: {Input.Email}");
                    return Page();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during login process");
                    ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                    return Page();
                }
            }

            return Page();
        }
    }
}