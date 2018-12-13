// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System.Security.Claims;

    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst("sub")?.Value ?? string.Empty;
        }

        public static string GetUserPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst("phone_number")?.Value;
        }
    }
}
