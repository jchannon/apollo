// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification.Email
{
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using IdentityModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class EmailVerificationModule : CarterModule
    {
        private readonly MailSender sender;

        private readonly ILogger<EmailVerificationModule> logger;

        private readonly IroncladClaimUpdater ironcladClaimUpdater;

        private readonly VerificationCodeManager verificationCodeManager;

        public EmailVerificationModule(VerificationCodeManager verificationCodeManager, MailSender sender, ILogger<EmailVerificationModule> logger, IroncladClaimUpdater ironcladClaimUpdater)
            : base("/emailverification")
        {
            this.verificationCodeManager = verificationCodeManager;
            this.sender = sender;
            this.logger = logger;
            this.ironcladClaimUpdater = ironcladClaimUpdater;

            this.RequiresAuthentication();

            this.Post("/", this.SendConfirmationCode);

            this.Post("/confirmation", this.ConfirmVerificationCode);
        }

        private async Task SendConfirmationCode(HttpContext context)
        {
            var userId = context.User.GetUserId();

            if (context.User.IsEmailVerified())
            {
                this.logger.LogInformation("User {userId} tried to verify their email when it's already verified", userId);

                context.Response.StatusCode = 204;
                return;
            }

            await this.verificationCodeManager.GenerateCode(VerificationType.Email, userId, async code =>
            {
                await this.sender.SendConfirmationCode(context.User.GetEmail(), code);
            });

            context.Response.StatusCode = 202;
        }

        private async Task ConfirmVerificationCode(HttpContext context)
        {
            var (validationResult, data) = context.Request.BindAndValidate<EmailConfirmationCodeModel>();

            if (!validationResult.IsValid)
            {
                this.logger.LogInformation("User sent an invalid payload ({validationResult}) when trying to verify the code", validationResult);
                context.Response.StatusCode = 422;
                await context.Response.WriteProblemDetails(new ValidationProblemDetails(validationResult));
                return;
            }

            var userId = context.User.GetUserId();

            await this.verificationCodeManager.VerifyCode(VerificationType.Email, userId, new VerificationCode(data.Code));

            await this.ironcladClaimUpdater.UpdateIronclad(userId, JwtClaimTypes.EmailVerified, true);

            context.Response.StatusCode = 204;
        }
    }
}
