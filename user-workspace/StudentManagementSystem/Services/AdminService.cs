using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Enums;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Services
{
    public interface IAdminService
    {
        Task<Admin> GetAdminAsync(int adminId);
        Task<Admin> CreateAdminAsync(Admin admin);
        Task<bool> UpdateAdminAsync(Admin admin);
        Task<bool> DeleteAdminAsync(int adminId);
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<bool> UpdatePermissionsAsync(int adminId, Dictionary<string, bool> permissions);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeAdminId = null);
        Task<Admin?> GetAdminByEmailAsync(string email);
        Task<bool> ValidateCredentialsAsync(string email, string password);
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<bool> UpdateCourseFeeAsync(int courseId, decimal newFee);
        Task<IEnumerable<StudentIssue>> GetStudentIssuesAsync(string? status);
        Task<bool> UpdateIssueStatusAsync(int issueId, string status, string? resolution);
     
        Task<IEnumerable<CourseSchedule>> GetAllTimetablesAsync();
    
        Task<IEnumerable<StudentIssue>> GetStudentIssuesAsync();
        Task<CourseSchedule?> GetStudentTimetableAsync(string studentId);
       
    }

    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _logger;

        public AdminService(ApplicationDbContext context, ILoggingService logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Admin> GetAdminAsync(int adminId)
        {
            var admin = await _context.Admins.FindAsync(adminId);
            if (admin == null)
            {
                throw new NotFoundException($"Admin {adminId} not found");
            }
            return admin;
        }

        public async Task<Admin> CreateAdminAsync(Admin admin)
        {
            if (await IsEmailUniqueAsync(admin.Email))
            {
                admin.CreatedDate = DateTime.UtcNow;
                admin.LastLoginDate = DateTime.UtcNow;

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New admin created: {admin.AdminId}");
                return admin;
            }

            throw new DuplicateException("Email address is already in use");
        }

        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            var existingAdmin = await GetAdminAsync(admin.AdminId);

            if (!await IsEmailUniqueAsync(admin.Email, admin.AdminId))
            {
                throw new DuplicateException("Email address is already in use");
            }

            existingAdmin.FirstName = admin.FirstName;
            existingAdmin.LastName = admin.LastName;
            existingAdmin.Email = admin.Email;
            existingAdmin.PhoneNumber = admin.PhoneNumber;
            existingAdmin.Role = admin.Role;
            existingAdmin.Department = admin.Department;
            existingAdmin.IsActive = admin.IsActive;
            existingAdmin.Notes = admin.Notes;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Admin updated: {admin.AdminId}");
            return true;
        }

        public async Task<bool> DeleteAdminAsync(int adminId)
        {
            var admin = await GetAdminAsync(adminId);

            if (admin.Role == "SuperAdmin")
            {
                throw new AppException("Cannot delete super admin account");
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Admin deleted: {adminId}");
            return true;
        }

        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _context.Admins
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToListAsync();
        }

        public async Task<bool> UpdatePermissionsAsync(int adminId, Dictionary<string, bool> permissions)
        {
            var admin = await GetAdminAsync(adminId);

            if (admin.Role == "SuperAdmin")
            {
                throw new AppException("Cannot modify super admin permissions");
            }

            using var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                admin.CanManageStudents = permissions.TryGetValue("CanManageStudents", out var manageStudents) ? manageStudents : admin.CanManageStudents;
                admin.CanManageCourses = permissions.TryGetValue("CanManageCourses", out var manageCourses) ? manageCourses : admin.CanManageCourses;
                admin.CanManagePayments = permissions.TryGetValue("CanManagePayments", out var managePayments) ? managePayments : admin.CanManagePayments;
                admin.CanManageAdmins = permissions.TryGetValue("CanManageAdmins", out var manageAdmins) ? manageAdmins : admin.CanManageAdmins;
                admin.CanViewReports = permissions.TryGetValue("CanViewReports", out var viewReports) ? viewReports : admin.CanViewReports;
                admin.CanManageSettings = permissions.TryGetValue("CanManageSettings", out var manageSettings) ? manageSettings : admin.CanManageSettings;

                await _context.SaveChangesAsync();
                scope.Complete();
                _logger.LogInformation($"Permissions updated for admin: {adminId}");
                return true;
            }
            catch (Exception)
            {
                // Transaction will automatically rollback
                throw;
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeAdminId = null)
        {
            return !await _context.Admins
                .AnyAsync(a => a.Email == email && a.AdminId != excludeAdminId);
        }

        public async Task<Admin?> GetAdminByEmailAsync(string email)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var admin = await GetAdminByEmailAsync(email);
            if (admin == null)
            {
                return false;
            }

            // Here you would validate the hashed password
            // For demo purposes, we're not implementing actual password validation
            return true;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .OrderBy(c => c.CourseCode)
                .ToListAsync();
        }

        public async Task<bool> UpdateCourseFeeAsync(int courseId, decimal newFee)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                throw new NotFoundException($"Course {courseId} not found");
            }

            course.Fee = newFee;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Course fee updated for course: {courseId}");
            return true;
        }

        public async Task<IEnumerable<StudentIssue>> GetStudentIssuesAsync()
        {
            return await _context.StudentIssues
                .Include(i => i.Student)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateIssueStatusAsync(int issueId, string status)
        {
            var issue = await _context.StudentIssues.FindAsync(issueId);
            if (issue == null)
            {
                throw new NotFoundException($"Issue {issueId} not found");
            }

            issue.Status = status;
            issue.UpdatedAt = DateTime.UtcNow;
            if (status == "Resolved")
            {
                issue.DateResolved = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Issue status updated for issue: {issueId}");
            return true;
        }

        public async Task<CourseSchedule> GetStudentTimetableAsync(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.CourseEnrollments)
                .ThenInclude(ce => ce.Course)
                .ThenInclude(c => c.CourseSchedules)
                .FirstOrDefaultAsync(s => s.StudentId == studentId.ToString());

            if (student == null)
            {
                throw new NotFoundException($"Student {studentId} not found");
            }

            // For simplicity, returning the first course schedule
            // In a real application, you'd want to return all schedules or handle this differently
            return student.CourseEnrollments
                .FirstOrDefault()?.Course?.CourseSchedules
                .FirstOrDefault() ?? throw new NotFoundException("No timetable found for student");
        }

        public async Task<IEnumerable<CourseSchedule>> GetAllTimetablesAsync()
        {
            return await _context.CourseSchedules
                .Include(cs => cs.Course)
                .OrderBy(cs => cs.Course.CourseName)
                .ThenBy(cs => cs.StartTime)
                .ToListAsync();
        }


        Task<bool> IAdminService.UpdateIssueStatusAsync(int issueId, string status, string? resolution)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StudentIssue>> GetStudentIssuesAsync(string? status = null)
        {
            var query = _context.StudentIssues.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(i => i.Status == status);
            }

            return await query.Include(i => i.Student).ToListAsync();
        }

        Task<CourseSchedule?> IAdminService.GetStudentTimetableAsync(string studentId)
        {
            throw new NotImplementedException();
        }
    }
}