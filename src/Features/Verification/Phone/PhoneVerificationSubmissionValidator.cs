// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification.Phone
{
    using FluentValidation;

    public class PhoneVerificationSubmissionValidator : AbstractValidator<PhoneVerificationSubmission>
    {
        public PhoneVerificationSubmissionValidator()
        {
            this.RuleFor(x => x.Code)
                .Must(VerificationCode.IsWellformed)
                .WithMessage("The verification code is not well formed, meaning not a 4 digit code.");
        }
    }
}
