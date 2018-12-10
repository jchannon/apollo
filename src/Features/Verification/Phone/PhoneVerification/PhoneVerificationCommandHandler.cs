namespace Apollo.Features.Verification.Phone.PhoneVerification
{
    using System;
    using System.Threading.Tasks;
    using Apollo.Persistence;

    public class PhoneVerificationCommandHandler : CommandHandler<PhoneVerficationCommand>
    {
        private readonly IVerificationRequestRepository verificationRequestRepository;

        public PhoneVerificationCommandHandler(IVerificationRequestRepository verificationRequestRepository)
        {
            this.verificationRequestRepository = verificationRequestRepository;
        }

        protected override async Task<Error> Handle(PhoneVerficationCommand command)
        {
            if (!bool.Parse(command.User.FindFirst("email_verified").Value))
            {
                return new Error { ErrorCode = 400 };
            }

            var result = await this.verificationRequestRepository.GetVerificationRequest(VerificationType.SMS, command.User.GetUserId());

            if (result != null)
            {
                return new Error { ErrorCode = 400 };
            }

            //await this.verificationRequestRepository.(VerificationRequestDto.CreatePhoneVerificationRequest(command.User.GetUserId(), DateTime.UtcNow.AddMinutes(5),
              //  VerificationRequestMethod.SendSms));
            return null;
        }
    }
}
