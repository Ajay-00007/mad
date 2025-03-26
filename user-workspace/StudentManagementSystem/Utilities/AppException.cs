namespace StudentManagementSystem.Utilities
{
    public class AppException : Exception
    {
        public string? ErrorCode { get; }

        public AppException() : base() { }

        public AppException(string message) : base(message) { }

        public AppException(string message, Exception innerException) : base(message, innerException) { }

        public AppException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    public class AuthenticationException : AppException
    {
        public AuthenticationException(string message) : base(message, "AUTH_ERROR") { }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, "NOT_FOUND") { }
    }

    public class DuplicateException : AppException
    {
        public DuplicateException(string message) : base(message, "DUPLICATE") { }
    }

    public class ValidationException : AppException
    {
        public ValidationException(string message) : base(message, "VALIDATION") { }
    }

    public class EnrollmentException : AppException
    {
        public EnrollmentException(string message, string errorCode) : base(message, errorCode) { }
    }

    public class PaymentException : AppException
    {
        public PaymentException(string message, string errorCode) : base(message, errorCode) { }
    }

    public class ConfigurationException : AppException
    {
        public ConfigurationException(string message) : base(message, "CONFIG_ERROR") { }
    }

    public class EmailException : AppException
    {
        public EmailException(string message) : base(message, "EMAIL_ERROR") { }
    }
}