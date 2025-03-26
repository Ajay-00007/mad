using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Enums;
using StudentManagementSystem.Services;
using StudentManagementSystem.Infrastructure;
using StudentManagementSystem.Extensions;

namespace StudentManagementSystem.Pages
{
    public class ProfileModel : BasePageModel
    {
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;

        public ProfileModel(
            ApplicationDbContext context,
            ILogger<ProfileModel> logger,
            IStudentService studentService,
            IEnrollmentService enrollmentService)
            : base(context, logger)
        {
            _studentService = studentService;
            _enrollmentService = enrollmentService;
        }

        public Student Student { get; set; }
        public IList<CourseEnrollment> ActiveEnrollments { get; set; } = new List<CourseEnrollment>();
        public IList<CourseEnrollment> CompletedEnrollments { get; set; } = new List<CourseEnrollment>();
        public int TotalCreditsEarned { get; set; }
        public int ActiveCoursesCount { get; set; }
        public int CompletedCoursesCount { get; set; }

        [BindProperty]
        public Student InputModel { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            try
            {
                Student = await _studentService.GetStudentByIdAsync(studentId);
                var enrollments = await _enrollmentService.GetStudentEnrollmentsAsync(studentId);

                ActiveEnrollments = enrollments
                    .Where(e => e.Status == EnrollmentStatus.Active)
                    .OrderBy(e => e.Course.StartDate)
                    .ToList();

                CompletedEnrollments = enrollments
                    .Where(e => e.Status == EnrollmentStatus.Completed)
                    .OrderByDescending(e => e.CompletionDate)
                    .ToList();

                ActiveCoursesCount = ActiveEnrollments.Count;
                CompletedCoursesCount = CompletedEnrollments.Count;
                TotalCreditsEarned = CompletedEnrollments.Sum(e => e.Course.Credits);

                InputModel = new Student
                {
                    FirstName = Student.FirstName,
                    LastName = Student.LastName,
                    Email = Student.Email,
                    PhoneNumber = Student.PhoneNumber,
                    Address = Student.Address,
                    EmergencyContact = Student.EmergencyContact,
                    EmergencyPhone = Student.EmergencyPhone
                };

                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while loading profile for student {StudentId}", studentId);
                SetErrorMessage("An error occurred while loading your profile. Please try again.");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var studentId = User.GetStudentId();
            if (studentId == null)
            {
                return RedirectToLoginPage();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var student = await _studentService.GetStudentByIdAsync(studentId);

                student.PhoneNumber = InputModel.PhoneNumber;
                student.Address = InputModel.Address;
                student.EmergencyContact = InputModel.EmergencyContact;
                student.EmergencyPhone = InputModel.EmergencyPhone;

                await _studentService.UpdateStudentAsync(student);
                SetSuccessMessage("Profile updated successfully.");

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while updating profile for student {StudentId}", studentId);
                SetErrorMessage("An error occurred while updating your profile. Please try again.");
                return RedirectToPage();
            }
        }
    }
}