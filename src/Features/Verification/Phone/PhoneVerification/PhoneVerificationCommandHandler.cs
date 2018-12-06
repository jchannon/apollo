namespace Apollo.Features.Verification.Phone.PhoneVerification
{
    using System.Threading.Tasks;

    public class PhoneVerificationCommandHandler : CommandHandler<PhoneVerficationCommand>
    {
        protected override async Task<Error> Handle(PhoneVerficationCommand command)
        {
            if (!bool.Parse(command.User.FindFirst("email_verified").Value))
            {
                return new Error { ErrorCode = 400 };
            }

            return null;
        }
    }
}
