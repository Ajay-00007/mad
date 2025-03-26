using System.Security.Claims;

namespace StudentManagementSystem.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetStudentId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value;
        }

        public static string? GetUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            var claim = principal.FindFirst(ClaimTypes.Email);
            return claim?.Value;
        }

        public static string? GetFullName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            var firstName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = principal.FindFirst(ClaimTypes.Surname)?.Value;

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                return null;

            return $"{firstName} {lastName}".Trim();
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return false;

            return principal.IsInRole("Admin");
        }

        public static bool IsStudent(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return false;

            return principal.IsInRole("Student");
        }
    }
}