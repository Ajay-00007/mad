using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Extensions;

namespace StudentManagementSystem.Pages.Account
{
    public class UpdateBankDetailsModel : BasePageModel
    {
        private readonly ApplicationDbContext _context;

        public UpdateBankDetailsModel(ApplicationDbContext context, ILogger<UpdateBankDetailsModel> logger)
            : base(context, logger) 
        {
            _context = context;
        }

        [BindProperty]
        public BankDetails BankDetails { get; set; } = new BankDetails();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return NotFound();
            }

            BankDetails = await _context.BankDetails
                .FirstOrDefaultAsync(m => m.BankDetailsId == id && m.StudentId == studentId) 
                ?? new BankDetails();

            if (BankDetails == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return NotFound();
            }

            BankDetails.StudentId = studentId;
            BankDetails.LastUpdated = DateTime.UtcNow;

            if (BankDetails.BankDetailsId == 0)
            {
                _context.BankDetails.Add(BankDetails);
            }
            else
            {
                _context.Attach(BankDetails).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankDetailsExists(BankDetails.BankDetailsId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BankDetailsExists(int id)
        {
            return _context.BankDetails.Any(e => e.BankDetailsId == id);
        }
    }
}
