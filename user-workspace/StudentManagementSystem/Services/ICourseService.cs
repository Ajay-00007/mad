using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public interface ICourseService
    {
        Task<IList<Course>> GetAvailableCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int courseId);
        Task<bool> EnrollStudentAsync(string studentId, int courseId);
        Task<bool> DropCourseAsync(string studentId, int courseId);
        Task<IList<Course>> GetEnrolledCoursesAsync(string studentId);
        Task<IList<Course>> GetAllCoursesAsync();
        Task<bool> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int courseId);
        Task<bool> IsCourseAvailableAsync(int courseId);
        Task<bool> HasPrerequisitesAsync(string studentId, int courseId);
        Task<bool> IsEnrolledAsync(string studentId, int courseId);
        Task<int> GetEnrollmentCountAsync(int courseId);
        Task<decimal> GetCourseFeeAsync(int courseId);
        Task<IList<CourseSchedule>> GetCourseScheduleAsync(int courseId);
        Task<bool> HasScheduleConflictAsync(string studentId, int courseId);
        Task<bool> ValidateScheduleAsync(int courseId, DateTime startTime, DateTime endTime, DayOfWeek dayOfWeek);
        Task<bool> UpdateScheduleAsync(int courseId, IList<CourseSchedule> schedules);
        Task<Course> CreateCourseAsync(Course course);
        Task<IEnumerable<Course>> GetActiveCoursesAsync();
        Task<decimal> GetAverageRatingAsync(int courseId);
        Task<Course> GetCourseAsync(int courseId);
        Task<bool> IsCourseCodeUniqueAsync(string courseCode, int? excludeCourseId = null);
        Task<bool> UpdateScheduleAsync(int courseId, IEnumerable<CourseSchedule> schedules);
        
    }
}