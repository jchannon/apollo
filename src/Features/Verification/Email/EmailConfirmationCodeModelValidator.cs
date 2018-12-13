// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification.Email
{
    using FluentValidation;

    public class EmailConfirmationCodeModelValidator : AbstractValidator<EmailConfirmationCodeModel>
    {
        public EmailConfirmationCodeModelValidator()
        {
            this.RuleFor(x => x.Code).NotEmpty();
        }
    }
}
