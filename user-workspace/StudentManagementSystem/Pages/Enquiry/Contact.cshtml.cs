using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Enums;
using StudentManagementSystem.Services;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Extensions;

namespace StudentManagementSystem.Pages.Enquiry
{
    public class ContactModel : BasePageModel
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;

        public ContactModel(
            ApplicationDbContext context,
            ILogger<ContactModel> logger,
            INotificationService notificationService,
            IEmailService emailService)
            : base(context, logger)
        {
            _notificationService = notificationService;
            _emailService = emailService;
        }

        [BindProperty]
        public Models.Enquiry Enquiry { get; set; } = new Models.Enquiry();

        public List<Models.Enquiry> PreviousEnquiries { get; set; } = new List<Models.Enquiry>();

        public async Task<IActionResult> OnGetAsync()
        {
            var studentId = User.GetStudentId();
            if (studentId != null)
            {
                var student = await Context.Students
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (student != null)
                {
                    Enquiry.StudentId = studentId;
                    Enquiry.Name = student.FullName;
                    Enquiry.Email = student.Email;
                    Enquiry.Phone = student.PhoneNumber;
                }

               
                PreviousEnquiries = await Context.Enquiries
                    .Where(e => e.StudentId == studentId)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Enquiry.CreatedDate = DateTime.UtcNow;
                Enquiry.Status = EnquiryStatus.New;

                Context.Enquiries.Add(Enquiry);
                await Context.SaveChangesAsync();

               
                await _emailService.SendEnquiryConfirmationAsync(
                    Enquiry.Email,
                    Enquiry.Name,
                    Enquiry.Subject
                );

                
                await _notificationService.SendNewEnquiryNotificationAsync(
                    Enquiry.Subject,
                    Enquiry.Name,
                    Enquiry.Email
                );

                SetSuccessMessage("Your enquiry has been submitted successfully. We will contact you soon.");
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while submitting enquiry");
                SetErrorMessage("An error occurred while submitting your enquiry. Please try again.");
                return Page();
            }
        }
    }
}
