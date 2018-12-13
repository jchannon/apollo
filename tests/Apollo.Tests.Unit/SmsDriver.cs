// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Apollo.Features.Verification;
    using Apollo.Tests.Unit.Sdk;
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
            var request = await repo.GetVerificationRequest(VerificationType.SMS, this.UserId);

            return request.Code.ToString();
        }

        public async Task<HttpResponseMessage> SubmitVerificationCode(string verificationCode)
        {
            return await this.Services.ApolloClient.PostAsync(
                "/phoneverification/confirmation",
                new StringContent(JsonConvert.SerializeObject(new { code = verificationCode }), Encoding.UTF8, "application/json"));
        }

        public void CheckPhoneIsVerified()
        {
            throw new Exception("figure out how to check the status of phone verified claim");
        }
    }
}
