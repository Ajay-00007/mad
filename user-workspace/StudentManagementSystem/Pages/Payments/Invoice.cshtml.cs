using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Infrastructure;

namespace StudentManagementSystem.Pages.Payments
{
    public class InvoiceModel : BasePageModel
    {
        public Invoice? Invoice { get; set; }

        public InvoiceModel(ApplicationDbContext context, ILogger<InvoiceModel> logger)
            : base(context, logger)
        {
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (string.IsNullOrEmpty(CurrentStudentId))
            {
                return RedirectToLoginPage();
            }

            // Convert CurrentStudentId to int
            if (!int.TryParse(CurrentStudentId, out int studentId))
            {
                return BadRequest("Invalid Student ID.");
            }

            Invoice = await Context.Invoices
                .Include(i => i.Student)
                .Include(i => i.Payments)
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.EnrollmentId == studentId);

            if (Invoice == null)
            {
                return NotFound();
            }

            return Page();
        }


    }
}