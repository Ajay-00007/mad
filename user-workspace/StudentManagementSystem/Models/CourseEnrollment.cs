using System.ComponentModel.DataAnnotations;
using StudentManagementSystem.Models.Enums;

namespace StudentManagementSystem.Models
{
    public class CourseEnrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        public EnrollmentStatus Status { get; set; }

        public DateTime? CompletionDate { get; set; }

        public decimal? Grade { get; set; }

        public string? Notes { get; set; }

        public bool IsActive => Status == EnrollmentStatus.Active;

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual Course? Course { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<TeachingEvaluation> Evaluations { get; set; } = new List<TeachingEvaluation>();
        public DateTime DropDate { get; internal set; }
    }
}
