namespace Apollo.Tests.Unit
{
    using System.Collections.Generic;
    using IdentityModel;

    public class User
    {
        public Dictionary<string, string> Claims { get; set; }
    
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string UserId { get; set; }

        public User()
        {
            this.Claims = new Dictionary<string, string>();
        }

        public void VerifyEmail()
        {
            this.Claims.Add(JwtClaimTypes.EmailVerified, "true");
        }

        public void VerifyPhone()
        {
            this.Claims.Add(JwtClaimTypes.PhoneNumberVerified, "true");
        }
    }
}
