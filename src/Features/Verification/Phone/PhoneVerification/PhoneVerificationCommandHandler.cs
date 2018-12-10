namespace Apollo.Features.Verification.Phone.PhoneVerification
{
    using System.Threading.Tasks;

    public class PhoneVerificationCommandHandler : CommandHandler<PhoneVerficationCommand>
    {
        protected override Task<Error> Handle(PhoneVerficationCommand command)
        {
            if (!bool.Parse(command.User.FindFirst("email_verified").Value))
            {
                return Task.FromResult(new Error { ErrorCode = 400 });
            }

            return Task.FromResult<Error>(null);
        }
    }
}
