namespace Apollo.Features.Verification.Email
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;
    
    public class EmailVerificationModule : CarterModule
    {
        private readonly VerificationCodeManager verificationCodeManager;
        private readonly MailSender sender;
        private readonly HttpClient client;

        public EmailVerificationModule(VerificationCodeManager verificationCodeManager, MailSender sender) : base("/emailverification")
        {
            this.verificationCodeManager = verificationCodeManager;
            this.sender = sender;
            this.client = new HttpClient();
            
            this.RequiresAuthentication();

            this.Post("/", this.SendConfirmationCode);

            this.Post("/confirmation", this.ConfirmVerificationCode);
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

            await this.verificationCodeManager.VerifyCode(VerificationType.Email, userId, new VerificationCode(result.Data.Code));
            context.Response.StatusCode = 400;
        }

        private async Task SendConfirmationCode(HttpContext context)
        {
            var userId = context.User.GetUserId();

            await this.verificationCodeManager.GenerateCode(VerificationType.Email, userId, async code =>
            {
                this.sender.SendConfirmationCode(await this.GetUserEmail(context), code);
            });
            context.Response.StatusCode = 202;
        }

        private async Task<string> GetUserEmail(HttpContext context)
        {
            //todo remove when we get the claims from ironclad
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
