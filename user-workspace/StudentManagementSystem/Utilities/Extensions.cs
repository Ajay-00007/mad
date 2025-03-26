using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using StudentManagementSystem.Models;
using StudentManagementSystem.Models.Enums;

namespace StudentManagementSystem.Utilities
{
    public static class Extensions
    {
        // String Extensions
        public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);
        
        public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        // DateTime Extensions
        public static string ToFriendlyDate(this DateTime date)
        {
            return date.ToString(Constants.DateFormats.LongDate);
        }

        public static string ToFriendlyDateTime(this DateTime date)
        {
            return date.ToString(Constants.DateFormats.LongDateTime);
        }

        public static bool IsInPast(this DateTime date)
        {
            return date < DateTime.Now;
        }

        public static bool IsInFuture(this DateTime date)
        {
            return date > DateTime.Now;
        }

        // Enum Extensions
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), false)
                .FirstOrDefault() as System.ComponentModel.DisplayNameAttribute;
            return attribute?.DisplayName ?? value.ToString();
        }

        // IEnumerable Extensions
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            return source == null || !source.Any();
        }

        // SelectList Extensions
        public static SelectList ToSelectList<T>(this IEnumerable<T> items, string dataValueField, string dataTextField)
        {
            return new SelectList(items, dataValueField, dataTextField);
        }

        // ClaimsPrincipal Extensions
        public static string? GetEnrollmentId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string? GetStudentName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }

        // HttpContext Extensions
        public static string? GetEnrollmentId(this HttpContext context)
        {
            return context.Session.GetString(Constants.SessionKeys.EnrollmentId);
        }

        public static string? GetStudentName(this HttpContext context)
        {
            return context.Session.GetString(Constants.SessionKeys.StudentName);
        }

        // Model Extensions
        public static string GetStatusClass(this CourseEnrollment enrollment)
        {
            return enrollment.Status switch
            {
                EnrollmentStatus.Active => "success",
                EnrollmentStatus.Pending => "warning",
                EnrollmentStatus.Dropped => "danger",
                EnrollmentStatus.Completed => "info",
                _ => "secondary"
            };
        }

        public static string GetStatusClass(this Payment payment)
        {
            return payment.Status switch
            {
                PaymentStatus.Completed => "success",
                PaymentStatus.Pending => "warning",
                PaymentStatus.Failed => "danger",
                PaymentStatus.Refunded => "info",
                _ => "secondary"
            };
        }

        // Decimal Extensions
        public static string ToCurrency(this decimal amount)
        {
            return amount.ToString("C2");
        }

        // Collection Extensions
        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                destination.Add(item);
            }
        }

        // Dictionary Extensions
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue) where TKey : notnull
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        // Exception Extensions
        public static string GetFullMessage(this Exception ex)
        {
            var messages = new List<string>();
            var currentEx = ex;

            while (currentEx != null)
            {
                messages.Add(currentEx.Message);
                currentEx = currentEx.InnerException;
            }

            return string.Join(" -> ", messages);
        }
    }
}