using Microsoft.Extensions.Options;
using StudentManagementSystem.Configuration;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public interface INotificationService
    {
        Task SendPaymentReminderAsync(string email, string name, int invoiceId, DateTime dueDate, decimal amount);
        Task SendCourseStartReminderAsync(string email, string name, string courseName, DateTime startDate);
        Task SendEnrollmentDeadlineReminderAsync(string email, string name, string courseName, DateTime deadline);
        Task SendEvaluationReminderAsync(string email, string name, string courseName);
        Task SendPaymentConfirmationAsync(string email, string name, int invoiceId, decimal amount);
        Task SendEnrollmentConfirmationAsync(string email, string name, string courseName, DateTime startDate);
        Task SendPasswordResetAsync(string email, string name, string resetLink);
        Task SendWelcomeEmailAsync(string email, string name);
        Task SendLowSeatAlertAsync(string email, string name, string courseName, int seatsLeft);
        Task SendEnrollmentStatusUpdateAsync(string email, string name, string courseName, string status);
        Task SendEnrollmentNotificationAsync(string email, string name, string courseName);
        
        Task SendEnrollmentConfirmationAsync(string studentId, string courseName);
        Task SendCourseDropConfirmationAsync(string studentId, string courseName);
        Task SendNewEnquiryNotificationAsync(string studentId, string enquiryType, string message);
        Task SendGradeUpdateNotificationAsync(string studentId, string courseName, string grade);
        Task SendDueDateReminderAsync(string studentId, string assignmentName, DateTime dueDate);
        Task SendLowAttendanceWarningAsync(string studentId, string courseName, decimal attendancePercentage);
        Task SendFeesDueReminderAsync(string studentId, decimal amount, DateTime dueDate);
        Task SendScheduleChangeNotificationAsync(string studentId, string courseName, DateTime oldTime, DateTime newTime);
        Task SendSystemMaintenanceNotificationAsync(DateTime startTime, DateTime endTime);
        Task SendBulkNotificationAsync(IEnumerable<string> studentIds, string message);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string studentId);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task DeleteNotificationAsync(int notificationId);
        Task<bool> HasUnreadNotificationsAsync(string studentId);
      
    }

    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ILoggingService _logger;
        private readonly AppSettings _appSettings;

        public NotificationService(
            IEmailService emailService,
            ILoggingService logger,
            IOptions<AppSettings> appSettings)
        {
            _emailService = emailService;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task SendPaymentReminderAsync(string email, string name, int invoiceId, DateTime dueDate, decimal amount)
        {
            try
            {
                var subject = $"Payment Reminder - Invoice #{invoiceId}";
                var body = $@"
                    Dear {name},

                    This is a friendly reminder that payment for Invoice #{invoiceId} is due on {dueDate:MMMM dd, yyyy}.

                    Amount Due: {amount:C}

                    Please ensure timely payment to avoid any late fees or service interruptions.

                    You can make your payment through our student portal or contact the finance office for assistance.

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Payment reminder sent for Invoice #{InvoiceId} to {Email}", invoiceId, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payment reminder for Invoice #{InvoiceId} to {Email}", invoiceId, email);
                throw;
            }
        }

        public async Task SendCourseStartReminderAsync(string email, string name, string courseName, DateTime startDate)
        {
            try
            {
                var subject = $"Course Start Reminder - {courseName}";
                var body = $@"
                    Dear {name},

                    This is a reminder that your course '{courseName}' starts on {startDate:MMMM dd, yyyy}.

                    Please ensure you have:
                    - Completed all pre-course requirements
                    - Accessed the course materials
                    - Reviewed the course schedule

                    We look forward to seeing you in class!

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Course start reminder sent for {Course} to {Email}", courseName, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send course start reminder for {Course} to {Email}", courseName, email);
                throw;
            }
        }

        public async Task SendEnrollmentDeadlineReminderAsync(string email, string name, string courseName, DateTime deadline)
        {
            try
            {
                var subject = $"Enrollment Deadline Reminder - {courseName}";
                var body = $@"
                    Dear {name},

                    The enrollment deadline for '{courseName}' is approaching ({deadline:MMMM dd, yyyy}).

                    Please complete your enrollment process before the deadline to secure your place in the course.

                    If you need any assistance, please contact our support team.

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Enrollment deadline reminder sent for {Course} to {Email}", courseName, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send enrollment deadline reminder for {Course} to {Email}", courseName, email);
                throw;
            }
        }

        public async Task SendEvaluationReminderAsync(string email, string name, string courseName)
        {
            try
            {
                var subject = $"Course Evaluation Reminder - {courseName}";
                var body = $@"
                    Dear {name},

                    We value your feedback! Please take a moment to complete the evaluation for '{courseName}'.

                    Your feedback helps us improve our courses and provide better learning experiences.

                    You can complete the evaluation through your student portal.

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Evaluation reminder sent for {Course} to {Email}", courseName, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send evaluation reminder for {Course} to {Email}", courseName, email);
                throw;
            }
        }

        public async Task SendPaymentConfirmationAsync(string email, string name, int invoiceId, decimal amount)
        {
            try
            {
                var subject = $"Payment Confirmation - Invoice #{invoiceId}";
                var body = $@"
                    Dear {name},

                    Thank you for your payment of {amount:C} for Invoice #{invoiceId}.

                    Your payment has been successfully processed and recorded in our system.

                    You can view your payment history and download receipts from your student portal.

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Payment confirmation sent for Invoice #{InvoiceId} to {Email}", invoiceId, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payment confirmation for Invoice #{InvoiceId} to {Email}", invoiceId, email);
                throw;
            }
        }

        public async Task SendEnrollmentConfirmationAsync(string email, string name, string courseName, DateTime startDate)
        {
            try
            {
                var subject = $"Enrollment Confirmation - {courseName}";
                var body = $@"
                    Dear {name},

                    Your enrollment in '{courseName}' has been confirmed!

                    Course Start Date: {startDate:MMMM dd, yyyy}

                    You can access your course materials and schedule through your student portal.

                    We look forward to having you in class!

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Enrollment confirmation sent for {Course} to {Email}", courseName, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send enrollment confirmation for {Course} to {Email}", courseName, email);
                throw;
            }
        }

        public async Task SendPasswordResetAsync(string email, string name, string resetLink)
        {
            try
            {
                var subject = "Password Reset Request";
                var body = $@"
                    Dear {name},

                    We received a request to reset your password.

                    To reset your password, please click the link below:
                    {resetLink}

                    This link will expire in 24 hours.

                    If you didn't request this change, please ignore this email or contact support.

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Password reset email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string email, string name)
        {
            try
            {
                var subject = "Welcome to Student Management System";
                var body = $@"
                    Dear {name},

                    Welcome to the Student Management System!

                    Your account has been successfully created. You can now:
                    - Browse and enroll in courses
                    - Access course materials
                    - View your schedule
                    - Make payments
                    - And much more!

                    If you need any assistance, please don't hesitate to contact our support team.

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Welcome email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", email);
                throw;
            }
        }

        public async Task SendLowSeatAlertAsync(string email, string name, string courseName, int seatsLeft)
        {
            try
            {
                var subject = $"Low Seats Alert - {courseName}";
                var body = $@"
                    Dear {name},

                    This is to inform you that {courseName} has only {seatsLeft} seats remaining.

                    Please take necessary action to manage enrollments.

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Low seats alert sent for {Course} to {Email}", courseName, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send low seats alert for {Course} to {Email}", courseName, email);
                throw;
            }
        }

        public async Task SendEnrollmentStatusUpdateAsync(string email, string name, string courseName, string status)
        {
            try
            {
                var subject = $"Enrollment Status Update - {courseName}";
                var body = $@"
                    Dear {name},

                    Your enrollment status for {courseName} has been updated to: {status}

                    If you have any questions, please contact our support team.

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Enrollment status update sent for {Course} to {Email}", courseName, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send enrollment status update for {Course} to {Email}", courseName, email);
                throw;
            }
        }

        public async Task SendEnrollmentNotificationAsync(string email, string name, string courseName)
        {
            try
            {
                var subject = $"New Enrollment - {courseName}";
                var body = $@"
                    Dear {name},

                    A new enrollment has been registered for {courseName}.

                    Please review the enrollment details in your admin dashboard.

                    Best regards,
                    Student Management System Team";

                await _emailService.SendEmailAsync(email, subject, body);
                _logger.LogInformation("Enrollment notification sent for {Course} to {Email}", courseName, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send enrollment notification for {Course} to {Email}", courseName, email);
                throw;
            }
        }

        public Task SendEnrollmentConfirmationAsync(string studentId, string courseName)
        {
            throw new NotImplementedException();
        }

        public Task SendCourseDropConfirmationAsync(string studentId, string courseName)
        {
            throw new NotImplementedException();
        }

        public Task SendNewEnquiryNotificationAsync(string studentId, string enquiryType, string message)
        {
            throw new NotImplementedException();
        }

        public Task SendGradeUpdateNotificationAsync(string studentId, string courseName, string grade)
        {
            throw new NotImplementedException();
        }

        public Task SendDueDateReminderAsync(string studentId, string assignmentName, DateTime dueDate)
        {
            throw new NotImplementedException();
        }

        public Task SendLowAttendanceWarningAsync(string studentId, string courseName, decimal attendancePercentage)
        {
            throw new NotImplementedException();
        }

        public Task SendFeesDueReminderAsync(string studentId, decimal amount, DateTime dueDate)
        {
            throw new NotImplementedException();
        }

        public Task SendScheduleChangeNotificationAsync(string studentId, string courseName, DateTime oldTime, DateTime newTime)
        {
            throw new NotImplementedException();
        }

        public Task SendSystemMaintenanceNotificationAsync(DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public Task SendBulkNotificationAsync(IEnumerable<string> studentIds, string message)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string studentId)
        {
            throw new NotImplementedException();
        }

        public Task MarkNotificationAsReadAsync(int notificationId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteNotificationAsync(int notificationId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasUnreadNotificationsAsync(string studentId)
        {
            throw new NotImplementedException();
        }
    }
}