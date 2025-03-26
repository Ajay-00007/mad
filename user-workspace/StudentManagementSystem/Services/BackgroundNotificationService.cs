using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StudentManagementSystem.Configuration;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentManagementSystem.Services
{
    public class BackgroundNotificationService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILoggingService _logger;
        private readonly AppSettings _appSettings;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

        public BackgroundNotificationService(
            IServiceProvider services,
            ILoggingService logger,
            IOptions<AppSettings> appSettings)
        {
            _services = services;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Notification Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessNotificationsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing notifications");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Background Notification Service is stopping");
        }

        private async Task ProcessNotificationsAsync(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            // Process payment reminders
            await ProcessPaymentRemindersAsync(dbContext, notificationService, stoppingToken);

            // Process course start reminders
            await ProcessCourseStartRemindersAsync(dbContext, notificationService, stoppingToken);

            // Process enrollment deadlines
            await ProcessEnrollmentDeadlinesAsync(dbContext, notificationService, stoppingToken);

            // Process evaluation reminders
            await ProcessEvaluationRemindersAsync(dbContext, notificationService, stoppingToken);
        }

        private async Task ProcessPaymentRemindersAsync(
            ApplicationDbContext dbContext,
            INotificationService notificationService,
            CancellationToken stoppingToken)
        {
            var dueDate = DateTime.UtcNow.AddDays(_appSettings.ReminderDaysBeforeDueDate);
            var unpaidInvoices = await dbContext.Invoices
                .Include(i => i.Student)
                .Where(i => i.DueDate <= dueDate && 
                           i.Status == Models.Enums.InvoiceStatus.Pending)
                .ToListAsync(stoppingToken);

            foreach (var invoice in unpaidInvoices)
            {
                await notificationService.SendPaymentReminderAsync(
                    invoice.Student.Email,
                    invoice.Student.FullName,
                    invoice.InvoiceId,
                    invoice.DueDate ?? DateTime.UtcNow,
                    invoice.TotalAmount
                );
            }
        }

        private async Task ProcessCourseStartRemindersAsync(
            ApplicationDbContext dbContext,
            INotificationService notificationService,
            CancellationToken stoppingToken)
        {
            var startDate = DateTime.UtcNow.AddDays(_appSettings.ReminderDaysBeforeCourseStart);
            var upcomingCourses = await dbContext.CourseEnrollments
                .Include(ce => ce.Student)
                .Include(ce => ce.Course)
                .Where(ce => ce.Course.StartDate <= startDate && 
                           ce.Status == Models.Enums.EnrollmentStatus.Active)
                .ToListAsync(stoppingToken);

            foreach (var enrollment in upcomingCourses)
            {
                await notificationService.SendCourseStartReminderAsync(
                    enrollment.Student.Email,
                    enrollment.Student.FullName,
                    enrollment.Course.CourseName,
                    enrollment.Course.StartDate
                );
            }
        }

        private async Task ProcessEnrollmentDeadlinesAsync(
            ApplicationDbContext dbContext,
            INotificationService notificationService,
            CancellationToken stoppingToken)
        {
            var deadlineDate = DateTime.UtcNow.AddDays(7);
            var courses = await dbContext.Courses
                .Include(c => c.CourseEnrollments)
                    .ThenInclude(ce => ce.Student)
                .Where(c => c.StartDate <= deadlineDate && c.IsActive)
                .ToListAsync(stoppingToken);

            foreach (var course in courses)
            {
                foreach (var enrollment in course.CourseEnrollments)
                {
                    await notificationService.SendEnrollmentDeadlineReminderAsync(
                        enrollment.Student.Email,
                        enrollment.Student.FullName,
                        course.CourseName,
                        course.StartDate
                    );
                }
            }
        }

        private async Task ProcessEvaluationRemindersAsync(
            ApplicationDbContext dbContext,
            INotificationService notificationService,
            CancellationToken stoppingToken)
        {
            var evaluationDate = DateTime.UtcNow.AddDays(-7);
            var completedCourses = await dbContext.CourseEnrollments
                .Include(ce => ce.Student)
                .Include(ce => ce.Course)
                .Where(ce => ce.CompletionDate.HasValue && 
                           ce.CompletionDate.Value <= evaluationDate &&
                           ce.Status == Models.Enums.EnrollmentStatus.Completed)
                .ToListAsync(stoppingToken);

            foreach (var enrollment in completedCourses)
            {
                await notificationService.SendEvaluationReminderAsync(
                    enrollment.Student.Email,
                    enrollment.Student.FullName,
                    enrollment.Course.CourseName
                );
            }
        }
    }
}