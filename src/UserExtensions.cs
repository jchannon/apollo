// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System.Security.Claims;
    using IdentityModel;

    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value ?? string.Empty;
        }

        public static string GetUserPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(JwtClaimTypes.PhoneNumber)?.Value;
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(JwtClaimTypes.Email)?.Value;
        }

        public static bool IsEmailVerified(this ClaimsPrincipal claimsPrincipal)
        {
            return bool.Parse(claimsPrincipal.FindFirst(JwtClaimTypes.EmailVerified)?.Value ?? "false");
        }

        public static bool IsUserPhoneVerified(this ClaimsPrincipal claimsPrincipal)
        {
            return bool.Parse(claimsPrincipal.FindFirst(JwtClaimTypes.PhoneNumberVerified)?.Value ?? "false");
        }
    }
}
