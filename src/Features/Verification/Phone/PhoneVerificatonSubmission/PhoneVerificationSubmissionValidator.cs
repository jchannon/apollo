namespace Apollo.Features.Verification.Phone.PhoneVerificatonSubmission
{
    using FluentValidation;

    public class PhoneVerificationSubmissionValidator : AbstractValidator<PhoneVerificationSubmission>
    {
        public PhoneVerificationSubmissionValidator()
        {
            this.RuleFor(x => x.Code).NotEmpty();
        }
    }
}
