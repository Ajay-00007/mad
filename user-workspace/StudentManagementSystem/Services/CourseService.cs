using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ApplicationDbContext context, ILogger<CourseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IList<Course>> GetAvailableCoursesAsync()
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.CourseSchedules)
                    .Where(c => c.IsActive && c.EnrollmentCapacity > c.CourseEnrollments.Count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available courses");
                throw;
            }
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.CourseSchedules)
                    .FirstOrDefaultAsync(c => c.CourseId == courseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course by id {CourseId}", courseId);
                throw;
            }
        }

        public async Task<bool> EnrollStudentAsync(string studentId, int courseId)
        {
            try
            {
                var course = await GetCourseByIdAsync(courseId);
                if (course == null) return false;

                var enrollment = new CourseEnrollment
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = Models.Enums.EnrollmentStatus.Active
                };

                _context.CourseEnrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling student {StudentId} in course {CourseId}", studentId, courseId);
                throw;
            }
        }

        public async Task<bool> DropCourseAsync(string studentId, int courseId)
        {
            try
            {
                var enrollment = await _context.CourseEnrollments
                    .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

                if (enrollment == null) return false;

                enrollment.Status = Models.Enums.EnrollmentStatus.Dropped;
                enrollment.DropDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dropping course {CourseId} for student {StudentId}", courseId, studentId);
                throw;
            }
        }

        public async Task<IList<Course>> GetEnrolledCoursesAsync(string studentId)
        {
            try
            {
                return await _context.CourseEnrollments
                    .Include(ce => ce.Course)
                        .ThenInclude(c => c.CourseSchedules)
                    .Where(ce => ce.StudentId == studentId && ce.Status == Models.Enums.EnrollmentStatus.Active)
                    .Select(ce => ce.Course)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrolled courses for student {StudentId}", studentId);
                throw;
            }
        }

        public async Task<IList<Course>> GetAllCoursesAsync()
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.CourseSchedules)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all courses");
                throw;
            }
        }

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            try
            {
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course {CourseId}", course.CourseId);
                throw;
            }
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            try
            {
                var course = await GetCourseByIdAsync(courseId);
                if (course == null) return false;

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course {CourseId}", courseId);
                throw;
            }
        }

        public async Task<bool> IsCourseAvailableAsync(int courseId)
        {
            try
            {
                var course = await GetCourseByIdAsync(courseId);
                if (course == null) return false;

                return course.IsActive && course.EnrollmentCapacity > course.CourseEnrollments.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking course availability {CourseId}", courseId);
                throw;
            }
        }

        public async Task<bool> HasPrerequisitesAsync(string studentId, int courseId)
        {
            try
            {
                var course = await GetCourseByIdAsync(courseId);
                if (course == null) return false;

                foreach (var prerequisiteId in course.Prerequisites)
                {
                    var completed = await _context.CourseEnrollments
                        .AnyAsync(ce => ce.StudentId == studentId &&
                                      ce.CourseId == prerequisiteId &&
                                      ce.Status == Models.Enums.EnrollmentStatus.Completed);

                    if (!completed) return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking prerequisites for student {StudentId} course {CourseId}", studentId, courseId);
                throw;
            }
        }

        public async Task<bool> IsEnrolledAsync(string studentId, int courseId)
        {
            try
            {
                return await _context.CourseEnrollments
                    .AnyAsync(ce => ce.StudentId == studentId &&
                                  ce.CourseId == courseId &&
                                  ce.Status == Models.Enums.EnrollmentStatus.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking enrollment for student {StudentId} course {CourseId}", studentId, courseId);
                throw;
            }
        }

        public async Task<int> GetEnrollmentCountAsync(int courseId)
        {
            try
            {
                return await _context.CourseEnrollments
                    .CountAsync(ce => ce.CourseId == courseId &&
                                    ce.Status == Models.Enums.EnrollmentStatus.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollment count for course {CourseId}", courseId);
                throw;
            }
        }

        public async Task<decimal> GetCourseFeeAsync(int courseId)
        {
            try
            {
                var course = await GetCourseByIdAsync(courseId);
                return course?.CourseFee ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course fee for course {CourseId}", courseId);
                throw;
            }
        }

        public async Task<IList<CourseSchedule>> GetCourseScheduleAsync(int courseId)
        {
            try
            {
                return await _context.CourseSchedules
                    .Where(cs => cs.CourseId == courseId)
                    .OrderBy(cs => cs.DayOfWeek)
                    .ThenBy(cs => cs.StartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course schedule for course {CourseId}", courseId);
                throw;
            }
        }

        public async Task<bool> HasScheduleConflictAsync(string studentId, int courseId)
        {
            try
            {
                var newCourseSchedules = await GetCourseScheduleAsync(courseId);
                var enrolledCourses = await GetEnrolledCoursesAsync(studentId);
                var currentSchedules = enrolledCourses.SelectMany(c => c.CourseSchedules).ToList();

                foreach (var currentSlot in currentSchedules)
                {
                    foreach (var newSlot in newCourseSchedules)
                    {
                        if (currentSlot.DayOfWeek == newSlot.DayOfWeek)
                        {
                            if ((newSlot.StartTime >= currentSlot.StartTime && newSlot.StartTime < currentSlot.EndTime) ||
                                (newSlot.EndTime > currentSlot.StartTime && newSlot.EndTime <= currentSlot.EndTime) ||
                                (newSlot.StartTime <= currentSlot.StartTime && newSlot.EndTime >= currentSlot.EndTime))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking schedule conflict for student {StudentId} course {CourseId}", studentId, courseId);
                throw;
            }
        }

        public async Task<bool> ValidateScheduleAsync(int courseId, DateTime startTime, DateTime endTime, DayOfWeek dayOfWeek)
        {
            try
            {
                var schedules = await GetCourseScheduleAsync(courseId);
                foreach (var schedule in schedules)
                {
                    if (schedule.DayOfWeek == dayOfWeek)
                    {
                        if ((startTime.TimeOfDay >= schedule.StartTime && startTime.TimeOfDay < schedule.EndTime) ||
                            (endTime.TimeOfDay > schedule.StartTime && endTime.TimeOfDay <= schedule.EndTime) ||
                            (startTime.TimeOfDay <= schedule.StartTime && endTime.TimeOfDay >= schedule.EndTime))
                        {
                            return false;
                        }

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating schedule for course {CourseId}", courseId);
                throw;
            }
        }

        public async Task<bool> UpdateScheduleAsync(int courseId, IList<CourseSchedule> schedules)
        {
            try
            {
                var course = await GetCourseByIdAsync(courseId);
                if (course == null) return false;

                _context.CourseSchedules.RemoveRange(course.CourseSchedules);

                foreach (var schedule in schedules)
                {
                    schedule.CourseId = courseId;
                    _context.CourseSchedules.Add(schedule);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating schedule for course {CourseId}", courseId);
                throw;
            }
        }

        Task<Course> ICourseService.CreateCourseAsync(Course course)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Course>> ICourseService.GetActiveCoursesAsync()
        {
            throw new NotImplementedException();
        }

        Task<decimal> ICourseService.GetAverageRatingAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        Task<Course> ICourseService.GetCourseAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        Task<bool> ICourseService.IsCourseCodeUniqueAsync(string courseCode, int? excludeCourseId)
        {
            throw new NotImplementedException();
        }

        Task<bool> ICourseService.UpdateScheduleAsync(int courseId, IEnumerable<CourseSchedule> schedules)
        {
            throw new NotImplementedException();
        }
    }
}