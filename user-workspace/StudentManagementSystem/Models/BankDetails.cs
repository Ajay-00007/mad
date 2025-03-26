using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class BankDetails
    {
        [Key]
        public int BankDetailsId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string BranchCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string AccountHolderName { get; set; } = string.Empty;

        [StringLength(50)]
        public string SwiftCode { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public bool IsVerified { get; set; }

        public DateTime? VerificationDate { get; set; }

        // Navigation property
        public virtual Student? Student { get; set; }
    }
}