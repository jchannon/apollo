// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Apollo.Features.Verification;
    using Apollo.Tests.Unit.Sdk;
    using IdentityModel;
    using Newtonsoft.Json;

    public class SmsDriver : IdentityTestDriver
    {
        public SmsDriver(ApolloIntegrationFixture services)
            : base(services)
        {
        }

        public async Task<HttpResponseMessage> RequestSMSCode()
        {
            return await this.Services.ApolloClient.PostAsync("/phoneverification", null);
        }

        public async Task<string> WaitForSMS()
        {
            var repo = new AzureInMemoryRepository();
            var request = await repo.GetVerificationRequest(VerificationType.SMS, this.CurrentUser.Id);

            return request.Code.ToString();
        }

        public async Task<HttpResponseMessage> SubmitVerificationCode(string verificationCode)
        {
            return await this.Services.ApolloClient.PostAsync(
                "/phoneverification/confirmation",
                new StringContent(JsonConvert.SerializeObject(new { code = verificationCode }), Encoding.UTF8, "application/json"));
        }

        public async Task<bool> CheckPhoneIsVerified()
        {
            var user = await this.Services.UsersClient.GetUserAsync(this.CurrentUser.Username);
            return user.Claims.ContainsKey(JwtClaimTypes.PhoneNumberVerified) && bool.Parse(user.Claims[JwtClaimTypes.PhoneNumberVerified].ToString());
        }
    }
}
