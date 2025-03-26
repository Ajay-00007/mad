using System.ComponentModel.DataAnnotations;
using StudentManagementSystem.Models.Enums;

namespace StudentManagementSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        
        [Required]
        public string StudentId { get; set; } = string.Empty;
        
        public int? InvoiceId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [Required]
        public PaymentMethod Method { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; }
        
        public DateTime PaymentDate { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;  // Changed from Notes to Description
        
        public string? TransactionId { get; set; }
        
        public string? PaymentReference { get; set; }
        
        public DateTime? ProcessedDate { get; set; }
        
        public string? ProcessedBy { get; set; }
        
        public string? FailureReason { get; set; }

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public PaymentMethod PaymentMethod { get; internal set; }
        public string Enrollment { get; internal set; }
        public string EnrollmentId { get; internal set; }
    }
}
