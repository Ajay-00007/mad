using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal CourseFee { get; set; }

        // Alias for CourseFee to maintain compatibility
        public decimal Fee 
        { 
            get => CourseFee; 
            set => CourseFee = value; 
        }

        [Required]
        public int Credits { get; set; }

        [Required]
        public int MaxSeats { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int DurationInWeeks { get; set; }

        [StringLength(100)]
        public string? Instructor { get; set; }

        public string? Prerequisites { get; set; }

        public string? LearningOutcomes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastUpdated { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();
        public virtual ICollection<CourseSchedule> CourseSchedules { get; set; } = new List<CourseSchedule>();
        public virtual ICollection<TeachingEvaluation> TeachingEvaluations { get; set; } = new List<TeachingEvaluation>();
        public int EnrollmentCapacity { get; internal set; }
    }
}