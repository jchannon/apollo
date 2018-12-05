using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using Apollo.Models;
using Apollo.Settings;
using Carter;
using Carter.ModelBinding;
using Carter.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Apollo.Modules
{
    public class EmailVerificationModule : CarterModule
    {
        private readonly AppSettings appSettings;
        private readonly HttpClient client = new HttpClient();
        private static readonly Dictionary<string, string> codes = new Dictionary<string, string>();

        public EmailVerificationModule(AppSettings appSettings) : base("/emailVerification")
        {
            this.appSettings = appSettings;
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

                var codeExists = codes.TryGetValue(userId, out var realCode);

                if (codeExists)
                {
                    if (realCode == result.Data.Code)
                    {
                        context.Response.StatusCode = 204;
                        return;
                    }
                }

                context.Response.StatusCode = 400;
            });
        }

        private async Task SendConfirmationCode(HttpContext context)
        {
            var code = VerificationCode.Generate().ToString();
            using (var message = new MailMessage("test@example.com", await this.GetUserEmail(context))
            {
                Body = code
            })
            {
                this.SendMailMessage(message);
            }

            codes.Add(context.User.FindFirst("sub").Value, code);
            context.Response.StatusCode = 202;
        }

        private void SendMailMessage(MailMessage message)
        {
            using (var client = new SmtpClient(
                this.appSettings.Smtp.Host,
                this.appSettings.Smtp.Port))
            {
                client.Send(message);
            }
        }

        private async Task<string> GetUserEmail(HttpContext context)
        {
            this.client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
            using (var response = await this.client.GetAsync($"{this.appSettings.IdentityServer.Authority}connect/userinfo", context.RequestAborted).ConfigureAwait(false))
            {
                var jsonClaims = await response.Content.ReadAsStringAsync();
                var claimsObject = JObject.Parse(jsonClaims);
                return claimsObject["email"].ToString();
            }
        }
    }
}
