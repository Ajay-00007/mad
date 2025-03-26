using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Services;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Data;

namespace StudentManagementSystem.Pages
{
    public class ContactModel : BasePageModel
    {
        private readonly IEmailService _emailService;

        public ContactModel(ApplicationDbContext context, ILogger<ContactModel> logger, IEmailService emailService)
            : base(context, logger)
        {
            _emailService = emailService;
        }

        [BindProperty]
        public ContactFormModel ContactForm { get; set; }

        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            if (!IsAuthenticated)
                return RedirectToPage("/Index");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

           

            SuccessMessage = "Your message has been sent successfully. Our support team will contact you soon.";
            ContactForm = new ContactFormModel(); // Clear the form

            return Page();
        }
    }

    public class ContactFormModel
    {
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
