namespace Apollo.Features.VerifyPhone.PhoneVerificatonSubmission
{
    using FluentValidation;

    public class VerificationSubmissiionValidator : AbstractValidator<VerificationSubmission>
    {
        public VerificationSubmissiionValidator()
        {
            this.RuleFor(x => x.Code).NotEmpty();
        }
    }
}