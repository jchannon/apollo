namespace Apollo.Features.Verification.Phone.PhoneVerification
{
    using System.Security.Claims;
    using System.Threading;

    public class PhoneVerficationCommand
    {
        public PhoneVerificationMessage PhoneVerificationMessage { get; }

        public ClaimsPrincipal User { get; }

        public CancellationToken CancellationToken { get; }

        public PhoneVerficationCommand(PhoneVerificationMessage phoneVerificationMessage, ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            this.PhoneVerificationMessage = phoneVerificationMessage;
            this.User = user;
            this.CancellationToken = cancellationToken;
        }
        

    }
}
