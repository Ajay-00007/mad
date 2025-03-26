using System.ComponentModel.DataAnnotations;
using StudentManagementSystem.Models.Enums;

namespace StudentManagementSystem.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        public int? EnrollmentId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal? AdjustmentAmount { get; set; }

        [Required]
        public decimal FinalAmount { get; set; }

        [Required]
        public InvoiceStatus Status { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual CourseEnrollment? Enrollment { get; set; }
        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public DateTime PaidDate { get; internal set; }
    }
}