namespace Apollo.Features.Verification.Phone
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using IdentityModel;

    public class PhoneModule : CarterModule
    {
        public PhoneModule(VerificationCodeManager verificationCodeManager, TwilioSender twilioSender) : base("/phoneverification")
        {
            this.RequiresAuthentication();

            this.Post("/", async context =>
            {
                if (!this.UserEmailVerified(context.User))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                if (this.UserPhoneVerified(context.User))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                var generatedSuccessfully = await verificationCodeManager.GenerateCode(VerificationType.SMS, context.User.GetUserId(), code =>
                {
                    twilioSender.Send(context.User.GetUserPhoneNumber(), code);
                    return Task.CompletedTask;
                });

                context.Response.StatusCode = generatedSuccessfully ? 202 : 400;
            });

            this.Post("/confirmation", async context =>
            {
                var result = context.Request.BindAndValidate<PhoneVerificationSubmission>();

                if (!result.ValidationResult.IsValid)
                {
                    context.Response.StatusCode = 422;
                }

                var userId = context.User.GetUserId();

                var success = await verificationCodeManager.VerifyCode(VerificationType.SMS, userId, new VerificationCode(result.Data.Code));

                context.Response.StatusCode = success ? 204 : 400;
            });
        }

        private bool UserEmailVerified(ClaimsPrincipal claimsPrincipal)
        {
            return bool.Parse(claimsPrincipal.FindFirst(JwtClaimTypes.EmailVerified)?.Value ?? "false");
        }

        private bool UserPhoneVerified(ClaimsPrincipal claimsPrincipal)
        {
            return bool.Parse(claimsPrincipal.FindFirst(JwtClaimTypes.PhoneNumberVerified)?.Value ?? "false");
        }
    }
}
