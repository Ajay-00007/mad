using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Enums;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Services
{
    public interface IEnrollmentService
    {
        Task<CourseEnrollment> EnrollStudentAsync(string studentId, int courseId);
        Task<bool> DropCourseAsync(int enrollmentId);
        Task<CourseEnrollment> GetEnrollmentAsync(int enrollmentId);
        Task<IEnumerable<CourseEnrollment>> GetStudentEnrollmentsAsync(string studentId);
        Task<bool> UpdateEnrollmentStatusAsync(int enrollmentId, EnrollmentStatus status);
        Task<bool> IsStudentEnrolledAsync(string studentId, int courseId);
        Task<int> GetActiveEnrollmentsCountAsync(int courseId);
        Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByStatusAsync(EnrollmentStatus status);
    }

    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ICourseService _courseService;
        private readonly ILoggingService _logger;

        public EnrollmentService(
            ApplicationDbContext context,
            INotificationService notificationService,
            ICourseService courseService,
            ILoggingService logger)
        {
            _context = context;
            _notificationService = notificationService;
            _courseService = courseService;
            _logger = logger;
        }

        public async Task<CourseEnrollment> EnrollStudentAsync(string studentId, int courseId)
        {
            var course = await _courseService.GetCourseAsync(courseId);
            var student = await _context.Students.FindAsync(studentId);
            var activeEnrollments = await GetActiveEnrollmentsCountAsync(courseId);

            if (activeEnrollments >= course.MaxSeats)
            {
                throw new EnrollmentException("Course is full", "COURSE_FULL");
            }

            if (await IsStudentEnrolledAsync(studentId, courseId))
            {
                throw new DuplicateException("Student is already enrolled in this course");
            }

            var enrollment = new CourseEnrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                Status = EnrollmentStatus.Active,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.CourseEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            await _notificationService.SendEnrollmentNotificationAsync(
                student.Email,
                student.FullName,
                course.CourseName
            );

            if (activeEnrollments + 1 >= course.MaxSeats - 5)
            {
                await _notificationService.SendLowSeatAlertAsync(
                    student.Email,
                    student.FullName,
                    course.CourseName,
                    course.MaxSeats - (activeEnrollments + 1)
                );
            }

            _logger.LogInformation($"Student {studentId} enrolled in course {courseId}");
            return enrollment;
        }

        public async Task<bool> DropCourseAsync(int enrollmentId)
        {
            var enrollment = await GetEnrollmentAsync(enrollmentId);
            var student = await _context.Students.FindAsync(enrollment.StudentId);
            var course = await _context.Courses.FindAsync(enrollment.CourseId);

            if (enrollment.Status != EnrollmentStatus.Active)
            {
                throw new EnrollmentException("Cannot drop an inactive enrollment", "INVALID_STATUS");
            }

            enrollment.Status = EnrollmentStatus.Dropped;
            enrollment.CompletionDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _notificationService.SendEnrollmentStatusUpdateAsync(
                student.Email,
                student.FullName,
                course.CourseName,
                EnrollmentStatus.Dropped.ToString()
            );

            _logger.LogInformation($"Student {enrollment.StudentId} dropped course {enrollment.CourseId}");
            return true;
        }

        public async Task<CourseEnrollment> GetEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _context.CourseEnrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

            if (enrollment == null)
            {
                throw new NotFoundException($"Enrollment {enrollmentId} not found");
            }

            return enrollment;
        }

        public async Task<IEnumerable<CourseEnrollment>> GetStudentEnrollmentsAsync(string studentId)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateEnrollmentStatusAsync(int enrollmentId, EnrollmentStatus status)
        {
            var enrollment = await GetEnrollmentAsync(enrollmentId);
            var student = await _context.Students.FindAsync(enrollment.StudentId);
            var course = await _context.Courses.FindAsync(enrollment.CourseId);
            
            if (enrollment.Status == status)
            {
                return false;
            }

            enrollment.Status = status;
            if (status == EnrollmentStatus.Completed || status == EnrollmentStatus.Dropped)
            {
                enrollment.CompletionDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await _notificationService.SendEnrollmentStatusUpdateAsync(
                student.Email,
                student.FullName,
                course.CourseName,
                status.ToString()
            );

            _logger.LogInformation($"Enrollment {enrollmentId} status updated to {status}");
            return true;
        }

        public async Task<bool> IsStudentEnrolledAsync(string studentId, int courseId)
        {
            return await _context.CourseEnrollments
                .AnyAsync(e => e.StudentId == studentId && 
                              e.CourseId == courseId && 
                              e.Status == EnrollmentStatus.Active);
        }

        public async Task<int> GetActiveEnrollmentsCountAsync(int courseId)
        {
            return await _context.CourseEnrollments
                .CountAsync(e => e.CourseId == courseId && 
                                e.Status == EnrollmentStatus.Active);
        }

        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByStatusAsync(EnrollmentStatus status)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Status == status)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();
        }
    }
}