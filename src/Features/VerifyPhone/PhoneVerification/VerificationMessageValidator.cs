namespace Apollo.Features.VerifyPhone.PhoneVerification
{
    using FluentValidation;

    public class VerificationMessageValidator : AbstractValidator<VerificationMessage>
    {
        public VerificationMessageValidator()
        {
            this.RuleFor(x => x.Phonenumber).NotEmpty();
        }
    }
}