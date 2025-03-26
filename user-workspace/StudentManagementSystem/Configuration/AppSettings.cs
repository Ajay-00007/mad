namespace StudentManagementSystem.Configuration
{
    public class AppSettings
    {
        public string ApplicationName { get; set; } = "Student Management System";
        public FileStorageSettings FileStorage { get; set; }
        public int ReminderDaysBeforeDueDate { get; set; } = 3;
        public int ReminderDaysBeforeCourseStart { get; set; } = 7;
        public string Version { get; set; } = "1.0.0";
        public string Environment { get; set; } = "Development";
        public bool EnableDebugMode { get; set; } = false;
        public int SessionTimeoutMinutes { get; set; } = 30;
        public string DefaultCulture { get; set; } = "en-US";
        public string DefaultTimeZone { get; set; } = "UTC";
        public string SupportEmail { get; set; } = "support@example.com";
        public string AdminEmail { get; set; } = "admin@example.com";
        public bool RequireEmailVerification { get; set; } = true;
        public bool EnableTwoFactorAuth { get; set; } = false;
        public int MaxLoginAttempts { get; set; } = 5;
        public int LockoutDurationMinutes { get; set; } = 30;
        public int PasswordExpiryDays { get; set; } = 90;
        public bool EnableAuditLogging { get; set; } = true;
        public string LogLevel { get; set; } = "Information";
        public string LogFilePath { get; set; } = "logs";
        public int MaxLogFileSize { get; set; } = 10; // In MB
        public int LogRetentionDays { get; set; } = 30;
        public bool EnableNotifications { get; set; } = true;
        public bool EnableEmailNotifications { get; set; } = true;
        public bool EnableSmsNotifications { get; set; } = false;
        public bool EnablePushNotifications { get; set; } = false;
        public int DefaultPageSize { get; set; } = 10;
        public int MaxPageSize { get; set; } = 100;
        public string DateFormat { get; set; } = "yyyy-MM-dd";
        public string TimeFormat { get; set; } = "HH:mm:ss";
        public string CurrencyCode { get; set; } = "USD";
        public string CurrencySymbol { get; set; } = "$";
        public int DecimalPlaces { get; set; } = 2;
        public string BasePath { get; set; }
        public long MaxFileSizeBytes { get; set; }


    }


    public class CacheSettings
    {
        public bool EnableCaching { get; set; } = true;
        public int DefaultCacheTimeMinutes { get; set; } = 60;
        public string CacheProvider { get; set; } = "Memory";
        public string RedisConnection { get; set; } = string.Empty;
        public bool EnableCompression { get; set; } = true;
        public int MaxCacheSize { get; set; } = 1024; // In MB
    }

    public class SecuritySettings
    {
        public bool EnableCors { get; set; } = true;
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
        public string[] AllowedHeaders { get; set; } = Array.Empty<string>();
        public string[] AllowedMethods { get; set; } = Array.Empty<string>();
        public bool AllowCredentials { get; set; } = true;
        public bool EnableXssProtection { get; set; } = true;
        public bool EnableHsts { get; set; } = true;
        public int HstsDays { get; set; } = 365;
        public bool RequireHttps { get; set; } = true;
    }
}
