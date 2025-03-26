using System.ComponentModel.DataAnnotations;
using StudentManagementSystem.Models.Enums;

namespace StudentManagementSystem.Models
{
    public class Student
    {
        [Key]
        public string StudentId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        public string? EmergencyContact { get; set; }
        
        public string? EmergencyPhone { get; set; }
        
        public DateTime EnrollmentDate { get; set; }
        
        public StudentStatus Status { get; set; } = StudentStatus.Active;

        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<StudentIssue> Issues { get; set; } = new List<StudentIssue>();
        public virtual ICollection<Enquiry> Enquiries { get; set; } = new List<Enquiry>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual BankDetails? BankDetails { get; set; }
    }
}
