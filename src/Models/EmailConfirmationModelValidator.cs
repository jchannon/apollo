using FluentValidation;

namespace Apollo.Models
{
    public class EmailConfirmationModelValidator : AbstractValidator<EmailConfirmationModel>
    {
        public EmailConfirmationModelValidator()
        {
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
