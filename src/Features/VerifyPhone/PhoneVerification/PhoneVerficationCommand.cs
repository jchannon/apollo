namespace Apollo
{
    using System.Threading;
    using Apollo.Features.VerifyPhone.PhoneVerification;
    using Microsoft.Extensions.Primitives;

    public class PhoneVerficationCommand
    {
        public VerificationMessage VerificationMessage { get; }

        public StringValues AuthorizationToken { get; }

        public CancellationToken CancellationToken { get; }

        public PhoneVerficationCommand(VerificationMessage verificationMessage, StringValues authorizationToken, CancellationToken cancellationToken)
        {
            this.VerificationMessage = verificationMessage;
            this.AuthorizationToken = authorizationToken;
            this.CancellationToken = cancellationToken;
        }
        

    }
}