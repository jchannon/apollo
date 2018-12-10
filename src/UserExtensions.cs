namespace Apollo
{
    using System.Security.Claims;

    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("sub")?.Value ?? string.Empty;
        }
    }
}
