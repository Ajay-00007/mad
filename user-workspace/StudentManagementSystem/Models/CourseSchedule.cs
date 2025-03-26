using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class CourseSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Room { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? EffectiveStartDate { get; set; }

        public DateTime? EffectiveEndDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation property
        public virtual Course? Course { get; set; }
       
    }
}
