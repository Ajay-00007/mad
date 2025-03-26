using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.ViewModels;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Services
{
    public interface IStudentService
    {
        Task<Student> GetStudentAsync(string studentId);
        Task<Student> CreateStudentAsync(RegisterViewModel model);
        Task<bool> UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(string studentId);
        Task<bool> ChangePasswordAsync(string studentId, string currentPassword, string newPassword);
        Task<bool> ValidateCredentialsAsync(string studentId, string password);
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<bool> UpdateProfileAsync(string studentId, Student updatedInfo);
        Task<bool> IsEmailUniqueAsync(string email, string? excludeStudentId = null);
        Task<Student> GetStudentByIdAsync(string studentId);
        Task<Student> GetStudentByEmailAsync(string email);
        Task<int> GetTotalStudentsCountAsync();
    }

    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _logger;

        public StudentService(ApplicationDbContext context, ILoggingService logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Student> GetStudentAsync(string studentId)
        {
            var student = await _context.Students
                .Include(s => s.CourseEnrollments)
                .Include(s => s.BankDetails)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                throw new NotFoundException($"Student {studentId} not found");
            }

            return student;
        }

        public async Task<Student> GetStudentByIdAsync(string studentId)
        {
            var student = await _context.Students
                .Include(s => s.CourseEnrollments)
                .Include(s => s.BankDetails)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                throw new NotFoundException($"Student {studentId} not found");
            }

            return student;
        }

        public async Task<Student> GetStudentByEmailAsync(string email)
        {
            var student = await _context.Students
                .Include(s => s.CourseEnrollments)
                .Include(s => s.BankDetails)
                .FirstOrDefaultAsync(s => s.Email == email);

            if (student == null)
            {
                throw new NotFoundException($"Student with email {email} not found");
            }

            return student;
        }

        public async Task<int> GetTotalStudentsCountAsync()
        {
            return await _context.Students.CountAsync();
        }

        public async Task<Student> CreateStudentAsync(RegisterViewModel model)
        {
            if (await IsEmailUniqueAsync(model.Email))
            {
                var studentId = GenerateStudentId();
                var student = new Student
                {
                    StudentId = studentId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    EnrollmentDate = DateTime.UtcNow
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New student created: {studentId}");
                return student;
            }

            throw new DuplicateException("Email address is already in use");
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            var existingStudent = await GetStudentAsync(student.StudentId);

            if (!await IsEmailUniqueAsync(student.Email, student.StudentId))
            {
                throw new DuplicateException("Email address is already in use");
            }

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.Email = student.Email;
            existingStudent.PhoneNumber = student.PhoneNumber;
            existingStudent.EmergencyContact = student.EmergencyContact;
            existingStudent.EmergencyPhone = student.EmergencyPhone;
            existingStudent.Address = student.Address;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Student updated: {student.StudentId}");
            return true;
        }

        public async Task<bool> DeleteStudentAsync(string studentId)
        {
            var student = await GetStudentAsync(studentId);

            if (student.CourseEnrollments.Any(e => e.Status == Models.Enums.EnrollmentStatus.Active))
            {
                throw new AppException("Cannot delete student with active enrollments");
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Student deleted: {studentId}");
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string studentId, string currentPassword, string newPassword)
        {
            var student = await GetStudentAsync(studentId);

            if (!await ValidateCredentialsAsync(studentId, currentPassword))
            {
                throw new AuthenticationException("Current password is incorrect");
            }

            // Here you would hash the new password before saving
            // For demo purposes, we're not implementing actual password hashing

            _logger.LogInformation($"Password changed for student: {studentId}");
            return true;
        }

public async Task<bool> ValidateCredentialsAsync(string studentId, string password)
{
    var student = await GetStudentAsync(studentId);

    // Assuming the password is stored in a hashed format
    // Here you would validate the hashed password
    // For demo purposes, let's assume we have a method to verify the password
    return VerifyPassword(student.PasswordHash, password);
}

private bool VerifyPassword(string hashedPassword, string password)
{
    // Implement your password verification logic here
    // This is a placeholder for actual password verification
    return hashedPassword == password; // Replace with actual hash comparison
}

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.CourseEnrollments)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        public async Task<bool> UpdateProfileAsync(string studentId, Student updatedInfo)
        {
            var student = await GetStudentAsync(studentId);

            if (!await IsEmailUniqueAsync(updatedInfo.Email, studentId))
            {
                throw new DuplicateException("Email address is already in use");
            }

            student.PhoneNumber = updatedInfo.PhoneNumber;
            student.EmergencyContact = updatedInfo.EmergencyContact;
            student.EmergencyPhone = updatedInfo.EmergencyPhone;
            student.Address = updatedInfo.Address;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Profile updated for student: {studentId}");
            return true;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, string? excludeStudentId = null)
        {
            return !await _context.Students
                .AnyAsync(s => s.Email == email && s.StudentId != excludeStudentId);
        }

        private string GenerateStudentId()
        {
            // Generate a unique student ID with format: YYYY-NNNNN
            var year = DateTime.UtcNow.Year;
            var random = new Random();
            var number = random.Next(10000, 99999);
            return $"{year}-{number}";
        }
    }
}