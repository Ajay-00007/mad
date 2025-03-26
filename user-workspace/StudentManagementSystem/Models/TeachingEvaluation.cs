using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class TeachingEvaluation
    {
        [Key]
        public int EvaluationId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        public int? EnrollmentId { get; set; }

        [Required]
        [Range(1, 5)]
        public int TeachingQuality { get; set; }

        [Required]
        [Range(1, 5)]
        public int CourseContent { get; set; }

        [Required]
        [Range(1, 5)]
        public int LearningResources { get; set; }

        [Required]
        [Range(1, 5)]
        public int AssessmentMethods { get; set; }

        [Required]
        [Range(1, 5)]
        public int TeacherSupport { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public bool IsAnonymous { get; set; }

        // Navigation properties
        public virtual Course? Course { get; set; }
        public virtual Student? Student { get; set; }
        public virtual CourseEnrollment? Enrollment { get; set; }
    }
}
