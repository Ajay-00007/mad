namespace StudentManagementSystem.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead => ReadAt.HasValue;
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
        public string? AdditionalData { get; set; }

        // Navigation properties
        public virtual Student? Student { get; set; }
    }

    public enum NotificationType
    {
        Payment = 0,
        Enrollment = 1,
        CourseDrop = 2,
        Enquiry = 3,
        Grade = 4,
        DueDate = 5,
        Attendance = 6,
        Fees = 7,
        Schedule = 8,
        System = 9,
        Other = 10
    }
}