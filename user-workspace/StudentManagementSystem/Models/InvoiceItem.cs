using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class InvoiceItem
    {
        [Key]
        public int InvoiceItemId { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? TaxAmount { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal Amount => TotalAmount;

        public string? Notes { get; set; }

        public int? CourseId { get; set; }

        // Navigation properties
        public virtual Invoice? Invoice { get; set; }
        public virtual Course? Course { get; set; }
    }
}
