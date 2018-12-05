using FluentValidation;

namespace Apollo.Features.Verification.Email
{
    public class EmailConfirmationModelValidator : AbstractValidator<EmailConfirmationModel>
    {
        public EmailConfirmationModelValidator()
        {
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
