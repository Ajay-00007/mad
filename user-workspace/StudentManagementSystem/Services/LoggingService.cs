using Microsoft.Extensions.Logging;

namespace StudentManagementSystem.Services
{
    public interface ILoggingService
    {
        void LogTrace(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
        void LogCritical(Exception exception, string message, params object[] args);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly IWebHostEnvironment _environment;

        public LoggingService(
            ILogger<LoggingService> logger,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void LogTrace(string message, params object[] args)
        {
            _logger.LogTrace(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            var errorId = Guid.NewGuid().ToString();
            var errorDetails = new
            {
                ErrorId = errorId,
                Message = message,
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace,
                InnerException = exception.InnerException?.Message,
                Environment = _environment.EnvironmentName,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogError(
                exception,
                "Error ID: {ErrorId}. {Message}",
                errorId,
                string.Format(message, args)
            );

            // In development, we might want to write to a local file
            if (_environment.IsDevelopment())
            {
                try
                {
                    var logPath = Path.Combine(_environment.ContentRootPath, "logs");
                    Directory.CreateDirectory(logPath);
                    var logFile = Path.Combine(logPath, $"error_{DateTime.UtcNow:yyyyMMdd}.log");
                    var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Error ID: {errorId}\n" +
                                 $"Message: {string.Format(message, args)}\n" +
                                 $"Exception: {exception}\n" +
                                 $"Stack Trace: {exception.StackTrace}\n" +
                                 "----------------------------------------\n";
                    
                    File.AppendAllText(logFile, logEntry);
                }
                catch
                {
                    // Ignore file writing errors in development
                }
            }
        }

        public void LogCritical(Exception exception, string message, params object[] args)
        {
            var errorId = Guid.NewGuid().ToString();
            var errorDetails = new
            {
                ErrorId = errorId,
                Message = message,
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace,
                InnerException = exception.InnerException?.Message,
                Environment = _environment.EnvironmentName,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogCritical(
                exception,
                "Critical Error ID: {ErrorId}. {Message}",
                errorId,
                string.Format(message, args)
            );

            // For critical errors, we might want to implement additional notification mechanisms
            // such as sending emails to administrators or creating incident tickets
            try
            {
                NotifyAdministrators(errorDetails).Wait();
            }
            catch
            {
                // Log but don't throw if notification fails
                _logger.LogWarning("Failed to send critical error notification");
            }
        }

        private async Task NotifyAdministrators(object errorDetails)
        {
            // In a real application, this would send notifications to administrators
            // For now, we'll just log the attempt
            _logger.LogInformation("Administrator notification would be sent for critical error");
        }
    }
}