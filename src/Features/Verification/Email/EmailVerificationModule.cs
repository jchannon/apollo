namespace Apollo.Features.Verification.Email
{
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using IdentityModel;
    using Microsoft.AspNetCore.Http;

    public class EmailVerificationModule : CarterModule
    {
        private readonly MailSender sender;

        private readonly VerificationCodeManager verificationCodeManager;

        public EmailVerificationModule(VerificationCodeManager verificationCodeManager, MailSender sender) : base("/emailverification")
        {
            this.verificationCodeManager = verificationCodeManager;
            this.sender = sender;

            this.RequiresAuthentication();

            this.Post("/", this.SendConfirmationCode);

            this.Post("/confirmation", this.ConfirmVerificationCode);
        }

        private async Task SendConfirmationCode(HttpContext context)
        {
            var userId = context.User.GetUserId();

            if (this.GetUserEmailVerified(context))
            {
                context.Response.StatusCode = 400;
                return;
            }

           var generatedSuccessfully = await this.verificationCodeManager.GenerateCode(VerificationType.Email, userId, code =>
            {
                this.sender.SendConfirmationCode(this.GetUserEmail(context), code);

                return Task.CompletedTask;
            });
            
            context.Response.StatusCode = generatedSuccessfully ? 202 : 400;
        }
        
        private async Task ConfirmVerificationCode(HttpContext context)
        {
            var result = context.Request.BindAndValidate<EmailConfirmationCodeModel>();

            if (!result.ValidationResult.IsValid)
            {
                context.Response.StatusCode = 422;
                await context.Response.Negotiate(result.ValidationResult.GetFormattedErrors());
                return;
            }

            var userId = context.User.GetUserId();

            var success = await this.verificationCodeManager.VerifyCode(VerificationType.Email, userId, new VerificationCode(result.Data.Code));

            context.Response.StatusCode = success ? 204 : 400;
        }

        private string GetUserEmail(HttpContext context)
        {
            return context.User.FindFirst(JwtClaimTypes.Email)?.Value;
        }

        private bool GetUserEmailVerified(HttpContext context)
        {
            return bool.Parse(context.User.FindFirst(JwtClaimTypes.EmailVerified)?.Value ?? "false");
        }
    }
}
