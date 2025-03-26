using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Administrator";

        public string? Department { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime LastLoginDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Additional properties for permissions and access control
        public bool CanManageStudents { get; set; } = true;
        public bool CanManageCourses { get; set; } = true;
        public bool CanManagePayments { get; set; } = true;
        public bool CanManageAdmins { get; set; } = false;
        public bool CanViewReports { get; set; } = true;
        public bool CanManageSettings { get; set; } = false;
        public string PhoneNumber { get; internal set; } = string.Empty;
    }
}