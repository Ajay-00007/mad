namespace StudentManagementSystem.Utilities
{
    public static class Constants
    {
        // Session Keys
        public static class SessionKeys
        {
            public const string EnrollmentId = "EnrollmentId";
            public const string StudentName = "StudentName";
            public const string ReturnUrl = "ReturnUrl";
            public const string LastActivity = "LastActivity";
        }

        // Authentication
        public static class Auth
        {
            public const int SessionTimeoutMinutes = 30;
            public const int MaxLoginAttempts = 5;
            public const int LockoutDurationMinutes = 15;
        }

        // Validation
        public static class Validation
        {
            public const int MinPasswordLength = 8;
            public const int MaxPasswordLength = 100;
            public const int MinNameLength = 2;
            public const int MaxNameLength = 100;
            public const int MaxEmailLength = 256;
            public const int MaxPhoneLength = 20;
            public const string EnrollmentIdPattern = @"^I\d{8}$";
            public const int MinAge = 16;
            public const int MaxAge = 100;
        }

        // Pagination
        public static class Pagination
        {
            public const int DefaultPageSize = 10;
            public const int MaxPageSize = 50;
            public const int PaginationRange = 5;
        }

        // File Upload
        public static class FileUpload
        {
            public const int MaxFileSizeMB = 10;
            public const string AllowedFileTypes = ".jpg,.jpeg,.png,.pdf,.doc,.docx";
            public const string UploadDirectory = "uploads";
        }

        // Payment
        public static class Payment
        {
            public const decimal MinimumPaymentAmount = 10.00M;
            public const string CurrencyCode = "USD";
            public const int PaymentReferenceLength = 15;
            public const int TransactionIdLength = 20;
        }

        // Course
        public static class Course
        {
            public const int MinCredits = 1;
            public const int MaxCredits = 6;
            public const int MinSeats = 5;
            public const int MaxSeats = 50;
            public const decimal MinFee = 100.00M;
            public const decimal MaxFee = 10000.00M;
        }

        // Error Messages
        public static class ErrorMessages
        {
            public const string InvalidLogin = "Invalid enrollment ID or date of birth.";
            public const string AccountLocked = "Your account has been temporarily locked. Please try again later.";
            public const string SessionExpired = "Your session has expired. Please log in again.";
            public const string UnauthorizedAccess = "You are not authorized to access this resource.";
            public const string InvalidFileType = "Invalid file type. Allowed types are: {0}";
            public const string FileSizeExceeded = "File size exceeds the maximum limit of {0}MB.";
            public const string CourseNotFound = "The requested course was not found.";
            public const string EnrollmentNotFound = "The requested enrollment was not found.";
            public const string DuplicateEnrollment = "You are already enrolled in this course.";
            public const string CourseFull = "This course is currently full.";
            public const string InsufficientFunds = "Insufficient funds to complete the payment.";
            public const string PaymentFailed = "Payment processing failed. Please try again.";
        }

        // Success Messages
        public static class SuccessMessages
        {
            public const string RegistrationSuccess = "Registration successful! Your enrollment ID is: {0}";
            public const string EnrollmentSuccess = "Successfully enrolled in the course.";
            public const string PaymentSuccess = "Payment processed successfully.";
            public const string ProfileUpdateSuccess = "Profile updated successfully.";
            public const string PasswordChangeSuccess = "Password changed successfully.";
        }

        // Date Formats
        public static class DateFormats
        {
            public const string ShortDate = "MM/dd/yyyy";
            public const string LongDate = "MMMM dd, yyyy";
            public const string ShortDateTime = "MM/dd/yyyy HH:mm";
            public const string LongDateTime = "MMMM dd, yyyy HH:mm:ss";
        }
    }
}