using FluentValidation;

namespace Apollo.Features.Verification.Email
{
    public class EmailConfirmationCodeModelValidator : AbstractValidator<EmailConfirmationCodeModel>
    {
        public EmailConfirmationCodeModelValidator()
        {
            this.RuleFor(x => x.Code).NotEmpty();
        }
    }
}
