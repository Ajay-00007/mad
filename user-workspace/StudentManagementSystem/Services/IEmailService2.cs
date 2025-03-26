
namespace StudentManagementSystem.Services
{
    public interface IEmailService2
    {
        Task SendAccountLockoutNotificationAsync(string email, string name);
        Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body);
        Task SendCourseDropConfirmationAsync(string email, string name, string courseName);
        Task SendDueDateReminderAsync(string email, string name, string assignmentName, DateTime dueDate);
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(string email, string subject, string message, bool isHtml = false);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath);
        Task SendEmailWithAttachmentAsync(string email, string subject, string message, string attachmentPath, string attachmentName);
        Task SendEnquiryConfirmationAsync(string email, string name, string enquiryId);
        Task SendEnrollmentConfirmationAsync(string email, string name, string courseName);
        Task SendFeesDueReminderAsync(string email, string name, decimal amount, DateTime dueDate);
        Task SendGradeNotificationAsync(string email, string name, string courseName, string grade);
        Task SendLowAttendanceWarningAsync(string email, string name, string courseName, decimal attendancePercentage);
        Task SendPasswordResetEmailAsync(string email, string resetToken);
        Task SendPaymentConfirmationAsync(string email, string name, decimal amount, string paymentId);
        Task SendScheduleChangeNotificationAsync(string email, string name, string courseName, string oldSchedule, string newSchedule);
        Task SendWelcomeEmailAsync(string email, string name);
    }
}