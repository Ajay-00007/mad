using System.ComponentModel.DataAnnotations;
using StudentManagementSystem.Models.Enums;

namespace StudentManagementSystem.Models
{
    public class Enquiry
    {
        [Key]
        public int EnquiryId { get; set; }

        public string? StudentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Alias for CreatedDate to maintain compatibility
        public DateTime CreatedAt 
        { 
            get => CreatedDate; 
            set => CreatedDate = value; 
        }

        public DateTime? ResponseDate { get; set; }

        public string? Response { get; set; }

        public string? RespondedBy { get; set; }

        [Required]
        public EnquiryStatus Status { get; set; } = EnquiryStatus.New;

        // Navigation property
        public virtual Student? Student { get; set; }
    }
}