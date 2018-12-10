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
