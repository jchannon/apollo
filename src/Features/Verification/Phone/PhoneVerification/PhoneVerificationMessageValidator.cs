namespace Apollo.Features.Verification.Phone.PhoneVerification
{
    using FluentValidation;

    public class PhoneVerificationMessageValidator : AbstractValidator<PhoneVerificationMessage>
    {
        public PhoneVerificationMessageValidator()
        {
            this.RuleFor(x => x.Phonenumber).NotEmpty();
        }
    }
}
