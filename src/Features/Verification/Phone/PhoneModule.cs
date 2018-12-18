// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification.Phone
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using IdentityModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class PhoneModule : CarterModule
    {
        private readonly VerificationCodeManager verificationCodeManager;

        private readonly TwilioSender twilioSender;

        private readonly ILogger<PhoneModule> logger;

        private readonly IroncladClaimUpdater ironcladClaimUpdater;

        public PhoneModule(VerificationCodeManager verificationCodeManager, TwilioSender twilioSender, ILogger<PhoneModule> logger, IroncladClaimUpdater ironcladClaimUpdater)
            : base("/phoneverification")
        {
            this.verificationCodeManager = verificationCodeManager;
            this.twilioSender = twilioSender;
            this.logger = logger;
            this.ironcladClaimUpdater = ironcladClaimUpdater;
            this.RequiresAuthentication();

            this.Post("/", this.SendSmsConfirmationCode);

            this.Post("/confirmation", this.ConfirmSmsConfirmationCode);
        }

        private async Task SendSmsConfirmationCode(HttpContext context)
        {
            var userId = context.User.GetUserId();

            if (!context.User.IsEmailVerified())
            {
                this.logger.LogInformation("User {userId} tried to verify their phone number without verifying their email", userId);

                context.Response.StatusCode = 400;
                return;
            }

            if (context.User.IsUserPhoneVerified())
            {
                this.logger.LogInformation("User {userId} tried to verify their phone number when it's already verified", userId);

                context.Response.StatusCode = 400;
                return;
            }

            var generatedSuccessfully = await this.verificationCodeManager.GenerateCode(VerificationType.SMS, userId, code =>
            {
                this.twilioSender.Send(context.User.GetUserPhoneNumber(), code);

                return Task.CompletedTask;
            });

            context.Response.StatusCode = generatedSuccessfully ? 202 : 400;
        }

        private async Task ConfirmSmsConfirmationCode(HttpContext context)
        {
            var (validationResult, data) = context.Request.BindAndValidate<PhoneVerificationSubmission>();

            if (!validationResult.IsValid)
            {
                this.logger.LogInformation("User sent an invalid payload ({validationResult}) when trying to verify the code", validationResult);

                context.Response.StatusCode = 422;
            }

            var userId = context.User.GetUserId();

            var success = await this.verificationCodeManager.VerifyCode(VerificationType.SMS, userId, new VerificationCode(data.Code));

            if (success)
            {
                success = await this.ironcladClaimUpdater.UpdateIronclad(userId, JwtClaimTypes.PhoneNumberVerified, true);
            }

            context.Response.StatusCode = success ? 204 : 400;
        }
    }
}
