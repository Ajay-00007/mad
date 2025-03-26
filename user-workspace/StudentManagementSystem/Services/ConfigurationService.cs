using Microsoft.Extensions.Options;
using StudentManagementSystem.Configuration;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Services
{
    public interface IConfigurationService
    {
        EmailSettings GetEmailSettings();
        AppSettings GetAppSettings();
        FileStorageSettings GetFileStorage();
        Task<bool> UpdateEmailSettingsAsync(EmailSettings settings);
        Task<bool> UpdateAppSettingsAsync(AppSettings settings);
        Task<bool> ValidateEmailSettingsAsync(EmailSettings settings);
        Task<bool> ValidateAppSettingsAsync(AppSettings settings);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILoggingService _logger;

        public ConfigurationService(
            IOptions<EmailSettings> emailSettings,
            IOptions<AppSettings> appSettings,
            ILoggingService logger)
        {
            _emailSettings = emailSettings;
            _appSettings = appSettings;
            _logger = logger;
        }

        public EmailSettings GetEmailSettings()
        {
            return _emailSettings.Value;
        }

        public AppSettings GetAppSettings()
        {
            return _appSettings.Value;
        }

        public FileStorageSettings GetFileStorage()
        {
            return _appSettings.Value.FileStorage;
        }

        public async Task<bool> UpdateEmailSettingsAsync(EmailSettings settings)
        {
            try
            {
                if (!await ValidateEmailSettingsAsync(settings))
                {
                    throw new ValidationException("Invalid email settings configuration");
                }

                // In a real application, you would update the settings in your configuration store
                // (e.g., database, configuration file, etc.)
                
                _logger.LogInformation("Email settings updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update email settings");
                throw new ConfigurationException("Failed to update email settings");
            }
        }

        public async Task<bool> UpdateAppSettingsAsync(AppSettings settings)
        {
            try
            {
                if (!await ValidateAppSettingsAsync(settings))
                {
                    throw new ValidationException("Invalid application settings configuration");
                }

                // In a real application, you would update the settings in your configuration store
                // (e.g., database, configuration file, etc.)

                _logger.LogInformation("Application settings updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update application settings");
                throw new ConfigurationException("Failed to update application settings");
            }
        }

        public async Task<bool> ValidateEmailSettingsAsync(EmailSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SmtpServer))
                return false;

            if (settings.SmtpPort <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(settings.SmtpUsername))
                return false;

            if (string.IsNullOrWhiteSpace(settings.SmtpPassword))
                return false;

            if (string.IsNullOrWhiteSpace(settings.FromEmail))
                return false;

            if (string.IsNullOrWhiteSpace(settings.FromName))
                return false;

            if (settings.Timeout <= 0)
                return false;

            return true;
        }

        public async Task<bool> ValidateAppSettingsAsync(AppSettings settings)
        {
            if (settings == null)
                return false;

            if (settings.FileStorage == null)
                return false;

            if (string.IsNullOrWhiteSpace(settings.FileStorage.BasePath))
                return false;

            if (settings.FileStorage.MaxFileSizeBytes <= 0)
                return false;

            return true;
        }
    }
}