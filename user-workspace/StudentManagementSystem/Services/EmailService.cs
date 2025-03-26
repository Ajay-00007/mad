using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using StudentManagementSystem.Configuration;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath);
        Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body);
        Task SendEnquiryConfirmationAsync(string email, string name, string subject);
        Task SendWelcomeEmailAsync(string email, string enrollmentId);
    }

    public class EmailService(
        IOptions<EmailSettings> emailSettings,
        ILoggingService logger) : IEmailService
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value;
        private readonly ILoggingService _logger = logger;
        private readonly string _smtpServer = "smtp.example.com"; 
        private readonly int _smtpPort = 587; 
        private readonly string _smtpUser = "saieleuri@gmail.com"; 
        private readonly string _smtpPassword = "gfzt zbdv jofx kbql";

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("Recipient email address cannot be empty.", nameof(to));
            }

            _logger.LogInformation($"Attempting to send email to: {to}");

            try
            {
                using var message = CreateEmailMessage(to, subject, body);
                await SendEmailMessageAsync(message);
                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw new AppException("Failed to send email", ex);
            }
        }


        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath)
        {
            try
            {
                using var message = CreateEmailMessage(to, subject, body);

                if (!File.Exists(attachmentPath))
                {
                    throw new AppException($"Attachment file not found: {attachmentPath}");
                }

                var attachment = new Attachment(attachmentPath);
                message.Attachments.Add(attachment);

                await SendEmailMessageAsync(message);
                _logger.LogInformation($"Email with attachment sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email with attachment to {to}");
                throw new AppException("Failed to send email with attachment", ex);
            }
        }

        public async Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body)
        {
            try
            {
                foreach (var recipient in recipients)
                {
                    using var message = CreateEmailMessage(recipient, subject, body);
                    await SendEmailMessageAsync(message);
                }
                _logger.LogInformation($"Bulk email sent successfully to {recipients.Count()} recipients");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send bulk email");
                throw new AppException("Failed to send bulk email", ex);
            }
        }

        private MailMessage CreateEmailMessage(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("Recipient email address cannot be empty.", nameof(to));
            }

            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, "Student Management System"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(to)); 
            return message;
        }


        private async Task SendEmailMessageAsync(MailMessage message)
        {
            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.EnableSsl = true;

                try
                {
                    await smtpClient.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email send failed: {ex.Message}");
                }
            }
        }

        public async Task SendWelcomeEmailAsync(string email, string name)
        {
            Console.WriteLine(email);
            var subject = "Welcome to Student Management System";
            var body = $"Hello {name},\n\nWelcome to our platform!";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var subject = "Password Reset Request";
            var body = $"Click the link to reset your password: https://example.com/reset-password?token={resetToken}";
            await SendEmailAsync(email, subject, body, true);
        }

        public async Task SendEnrollmentConfirmationAsync(string email, string name, string courseName)
        {
            var subject = "Enrollment Confirmation";
            var body = $"Dear {name},\n\nYou have successfully enrolled in {courseName}.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPaymentConfirmationAsync(string email, string name, decimal amount, string paymentId)
        {
            var subject = "Payment Confirmation";
            var body = $"Dear {name},\n\nWe received your payment of ₹{amount} (Payment ID: {paymentId}). Thank you!";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEnquiryConfirmationAsync(string email, string name, string enquiryId)
        {
            var subject = "Enquiry Received";
            var body = $"Dear {name},\n\nYour enquiry (ID: {enquiryId}) has been received. We will get back to you soon.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendCourseDropConfirmationAsync(string email, string name, string courseName)
        {
            var subject = "Course Drop Confirmation";
            var body = $"Dear {name},\n\nYou have successfully dropped the course {courseName}.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendGradeNotificationAsync(string email, string name, string courseName, string grade)
        {
            var subject = "Grade Notification";
            var body = $"Dear {name},\n\nYour grade for {courseName} is {grade}.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendScheduleChangeNotificationAsync(string email, string name, string courseName, string oldSchedule, string newSchedule)
        {
            var subject = "Schedule Change Notification";
            var body = $"Dear {name},\n\nThe schedule for {courseName} has changed from {oldSchedule} to {newSchedule}.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendLowAttendanceWarningAsync(string email, string name, string courseName, decimal attendancePercentage)
        {
            var subject = "Low Attendance Warning";
            var body = $"Dear {name},\n\nYour attendance in {courseName} is at {attendancePercentage}%. Please attend classes regularly.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendDueDateReminderAsync(string email, string name, string assignmentName, DateTime dueDate)
        {
            var subject = "Assignment Due Date Reminder";
            var body = $"Dear {name},\n\nYour assignment '{assignmentName}' is due on {dueDate:dddd, MMMM dd, yyyy}.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendFeesDueReminderAsync(string email, string name, decimal amount, DateTime dueDate)
        {
            var subject = "Fees Due Reminder";
            var body = $"Dear {name},\n\nYour fees of ₹{amount} is due on {dueDate:dddd, MMMM dd, yyyy}. Please make the payment.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendAccountLockoutNotificationAsync(string email, string name)
        {
            var subject = "Account Locked";
            var body = $"Dear {name},\n\nYour account has been locked due to multiple failed login attempts.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailAsync(string email, string subject, string message, bool isHtml = false)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = message,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(email);
            await SendEmailMessageAsync(mailMessage);
        }

        public async Task SendEmailWithAttachmentAsync(string email, string subject, string message, string attachmentPath, string attachmentName)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = message,
                IsBodyHtml = false
            };

            mailMessage.To.Add(email);
            mailMessage.Attachments.Add(new Attachment(attachmentPath) { Name = attachmentName });

            await SendEmailMessageAsync(mailMessage);
        }
    }
}