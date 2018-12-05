using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using Apollo.Settings;
using Carter;
using Carter.ModelBinding;
using Carter.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Apollo.Features.Verification.Email
{
    public class EmailVerificationModule : CarterModule
    {
        private readonly VerificationCodeManager verificationCodeManager;
        private readonly MailSender sender;
        private readonly HttpClient client = new HttpClient();

        public EmailVerificationModule(VerificationCodeManager verificationCodeManager, MailSender sender) : base("/emailVerification")
        {
            this.verificationCodeManager = verificationCodeManager;
            this.sender = sender;
            this.RequiresAuthentication();

            this.Post("/", this.SendConfirmationCode);

            this.Post("/confirmation", async context =>
            {
                var result = context.Request.BindAndValidate<EmailConfirmationModel>();

                if (!result.ValidationResult.IsValid)
                {
                    context.Response.StatusCode = 422;
                    await context.Response.Negotiate(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                var userId = context.User.FindFirst("sub").Value;

                await verificationCodeManager.VerifyCode(VerificationType.Email, userId, new VerificationCode(result.Data.Code));
                context.Response.StatusCode = 400;
            });
        }

        private async Task SendConfirmationCode(HttpContext context)
        {
            var userId = context.User.FindFirst("sub").Value;

            await this.verificationCodeManager.GenerateCode(VerificationType.Email, userId, async code =>
            {
                this.sender.SendConfirmationCode(await GetUserEmail(context), code);
            });
            context.Response.StatusCode = 202;
        }

        private async Task<string> GetUserEmail(HttpContext context)
        {
            this.client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
            using (var response = await this.client.GetAsync($"http://localhost:5005/connect/userinfo", context.RequestAborted).ConfigureAwait(false))
            {
                var jsonClaims = await response.Content.ReadAsStringAsync();
                var claimsObject = JObject.Parse(jsonClaims);
                return claimsObject["email"].ToString();
            }
        }
    }
}
