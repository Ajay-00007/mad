using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentManagementSystem.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            // Clear all session data
            HttpContext.Session.Clear();
            
            return RedirectToPage("/Index");
        }

        public IActionResult OnGet()
        {
            // Redirect to home if someone tries to access this page directly
            return RedirectToPage("/Index");
        }
    }
}