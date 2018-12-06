using System.Collections.Generic;
using IdentityModel;

namespace Apollo.Tests.Unit
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Dictionary<string, string> Claims = new Dictionary<string, string>();

        public void VerifyEmail()
        {
            Claims.Add(JwtClaimTypes.EmailVerified, "true");
        }

        public void VerifyPhone()
        {
            Claims.Add(JwtClaimTypes.PhoneNumberVerified, "true");
        }
    }
}
