namespace Apollo.Features.Verification.Phone
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
