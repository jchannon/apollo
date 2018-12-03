namespace Apollo.Features.VerifyPhone
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Apollo.Features.VerifyPhone.PhoneVerification;
    using Apollo.Features.VerifyPhone.PhoneVerificatonSubmission;
    using Carter;
    using Carter.ModelBinding;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;

    public class PhoneModule : CarterModule
    {
        private static readonly Dictionary<string, string> recordedPhoneNumbers = new Dictionary<string, string>();

        private readonly HttpClient client = new HttpClient();

        private bool emailVerified;

        public PhoneModule(AppSettings appSettings)
        {
            //this.RequiresAuthentication();

            this.Get("/status", context => Task.CompletedTask);

            this.Post("/phone-verification", async context =>
            {
                await this.GetUserClaims(appSettings, context);

                var result = context.Request.BindAndValidate<VerificationMessage>();

                if (!result.ValidationResult.IsValid)
                {
                    context.Response.StatusCode = 422;
                    return;
                }

                if (!this.emailVerified)
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                if (recordedPhoneNumbers.ContainsKey(result.Data.Phonenumber))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                recordedPhoneNumbers.Add(result.Data.Phonenumber, string.Empty);

                context.Response.StatusCode = 202;
                await context.Response.WriteAsync("Hello World");
            });

            this.Post("/phone-verification-submission", async context =>
            {
                var result = context.Request.BindAndValidate<VerificationSubmission>();

                if (!result.ValidationResult.IsValid)
                {
                    context.Response.StatusCode = 422;
                    return;
                }

                context.Response.StatusCode = 204;
            });
        }

        private async Task GetUserClaims(AppSettings appSettings, HttpContext context)
        {
            this.client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
            using (var response = await this.client.GetAsync($"{appSettings.IdentityServer.Authority}/connect/userinfo", context.RequestAborted).ConfigureAwait(false))
            {
                var jsonClaims = await response.Content.ReadAsStringAsync();
                var claimsObject = JObject.Parse(jsonClaims);
                this.emailVerified = bool.Parse(claimsObject["email_verified"].ToString());
            }
        }
    }
}
