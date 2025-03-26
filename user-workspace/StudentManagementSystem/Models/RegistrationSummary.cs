using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class RegistrationSummary
    {
        [Key]
        public int SummaryId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public string AcademicTerm { get; set; } = string.Empty;

        [Required]
        public DateTime RegistrationDate { get; set; }

        public int TotalCredits { get; set; }

        public decimal TotalFees { get; set; }

        public RegistrationStatus Status { get; set; }

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        public virtual ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
        public virtual ICollection<CourseSchedule> Schedules { get; set; } = new List<CourseSchedule>();
    }

    public enum RegistrationStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled,
        Completed
    }

    // Extension of CourseEnrollment to include registration summary
    public class RegistrationEnrollment
    {
        [Key]
        public int RegistrationEnrollmentId { get; set; }

        [Required]
        public int RegistrationSummaryId { get; set; }

        [Required]
        public int CourseEnrollmentId { get; set; }

        // Navigation properties
        [ForeignKey("RegistrationSummaryId")]
        public virtual RegistrationSummary? RegistrationSummary { get; set; }

        [ForeignKey("CourseEnrollmentId")]
        public virtual CourseEnrollment? CourseEnrollment { get; set; }
    }
}