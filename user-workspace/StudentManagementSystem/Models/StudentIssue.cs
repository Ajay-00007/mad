using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class StudentIssue
    {
        [Key]
        public int IssueId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DateResolved { get; set; }

        // Alias for DateResolved to maintain compatibility
        public DateTime? ResolvedAt 
        { 
            get => DateResolved; 
            set => DateResolved = value; 
        }

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } = "Open";

        public string? Resolution { get; set; }

        public string? AssignedTo { get; set; }

        [StringLength(50)]
        public string Priority { get; set; } = "Medium";

        [StringLength(50)]
        public string Category { get; set; } = "General";

        public string? Notes { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string? UpdatedBy { get; set; }

        // Navigation property
        public virtual Student? Student { get; set; }
    }
}