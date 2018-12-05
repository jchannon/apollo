using System.Security.Claims;

namespace Apollo
{
    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("sub")?.Value ?? string.Empty;
        }
    }
}
