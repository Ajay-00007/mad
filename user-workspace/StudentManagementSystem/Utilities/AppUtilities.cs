using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace StudentManagementSystem.Utilities
{
    public static class AppUtilities
    {
        // Generate a unique enrollment ID
        public static string GenerateEnrollmentId(DateTime dateOfBirth, int sequence)
        {
            return $"I{dateOfBirth:yyMMdd}{sequence:D2}";
        }

        // Format currency
        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("C2");
        }

        // Generate a random reference number for payments
        public static string GenerateReferenceNumber()
        {
            return $"PAY-{DateTime.Now:yyyyMMdd}-{Path.GetRandomFileName().Replace(".", "").Substring(0, 6).ToUpper()}";
        }

        // Validate email format
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use a simple regex pattern for email validation
                var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // Validate phone number format
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Remove any non-digit characters
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
            return digitsOnly.Length >= 10 && digitsOnly.Length <= 15;
        }

        // Calculate age from date of birth
        public static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
                age--;
            return age;
        }

        // Generate a hash for sensitive data
        public static string GenerateHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Format file size
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;
            
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        // Truncate text with ellipsis
        public static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }

        // Convert enum to select list items
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> EnumToSelectList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = e.ToString(),
                    Value = e.ToString()
                })
                .ToList();
        }

        // Get friendly time span
        public static string GetFriendlyTimeSpan(DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;

            if (span.TotalDays > 365)
                return $"{(int)(span.TotalDays / 365)} year{((int)(span.TotalDays / 365) == 1 ? "" : "s")} ago";
            if (span.TotalDays > 30)
                return $"{(int)(span.TotalDays / 30)} month{((int)(span.TotalDays / 30) == 1 ? "" : "s")} ago";
            if (span.TotalDays > 7)
                return $"{(int)(span.TotalDays / 7)} week{((int)(span.TotalDays / 7) == 1 ? "" : "s")} ago";
            if (span.TotalDays >= 1)
                return $"{(int)span.TotalDays} day{((int)span.TotalDays == 1 ? "" : "s")} ago";
            if (span.TotalHours >= 1)
                return $"{(int)span.TotalHours} hour{((int)span.TotalHours == 1 ? "" : "s")} ago";
            if (span.TotalMinutes >= 1)
                return $"{(int)span.TotalMinutes} minute{((int)span.TotalMinutes == 1 ? "" : "s")} ago";

            return "just now";
        }
    }
}