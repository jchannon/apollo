namespace Apollo.Tests.Unit
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Apollo.Tests.Unit.Sdk;
    using FluentAssertions;
    using Newtonsoft.Json;
    using Xbehave;
    using Xunit;

    [Collection(nameof(ApolloIntegrationCollection))]
    public class SmsVerificationScenarios
    {
        private readonly Driver driver;

        public SmsVerificationScenarios(ApolloIntegrationFixture services)
        {
            this.driver = new Driver(services);
        }

        [Scenario]
        public void Verifying_an_invalid_phone_number()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(true); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number with invalid data".x(async () => { await this.driver.RequestSMSCodeWithInvalidData(); });

            "Then I get a API response to indication validation failure".x(async () => { await this.driver.CheckInvalidResponse(); });
        }

        [Scenario]
        public void Verifying_a_phone_number()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(true); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I receive an SMS with the verification code in".x(async () => { await this.driver.WaitForSMS(); });

            "And I can submit that code to verify my phone number".x(async () => { await this.driver.SubmitVerificationCode(); });

            "Then my phone number is verified".x(() => { this.driver.CheckPhoneIsVerified(); });
        }

        [Scenario]
        public void Verifying_a_phone_number_when_already_asked_for_verification()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(true); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I request a code to be verified again".x(async () => { await this.driver.RequestSubsequentSMSCode(); });

            "Then the response shows a bad request".x(() => { this.driver.CheckPhoneIsAlreadyBeingProcessed(); });
        }

        [Scenario]
        public void Verifying_phone_number_without_requesting_a_code()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(true); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I send an invalid verification code without submitting one".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });

            "Then my phone number is not verified".x(() => { this.driver.CheckPhoneIsNotVerified(); });
        }

        [Scenario]
        public void Veryifying_phone_number_without_verified_email()
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCodeWithUnverifiedEmail(); });

            "Then my phone number is not verified".x(async () => { await this.driver.CheckPhoneIsNotVerifiedWithUnverifiedEmail(); });
        }

        //        [Scenario]
        //        public void Providing_incorrect_code_three_times()
        //        {
        //            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });
        //
        //            "And I can login as the user".x(async () => { await this.driver.Login(); });
        //
        //            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCode(); });
        //
        //            "Then I receive an SMS with the verification code in".x(async () => { await this.driver.WaitForSMS(); });
        //
        //            "And I send an invalid verification code".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });
        //
        //            "And I send an invalid verification code a second time".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });
        //
        //            "And I send an invalid verification code a third time".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });
        //
        //            "And then I use the expired valid code".x(async () => { await this.driver.SubmitExpiredVerificationCode(); });
        //
        //            "Then my phone number is not verified".x(async () => { await this.driver.CheckPhoneIsNotVerified(); });
        //        }

        //
        //
        //        [Scenario]
        //        public void Providing_incorrect_code_three_times_and_then_succeeding_with_new_code()
        //        {
        //            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });
        //
        //            "And I can login as the user".x(async () => { await this.driver.Login(); });
        //
        //            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCode(); });
        //
        //            "Then I receive an SMS with the verification code in".x(async () => { await this.driver.WaitForSMS(); });
        //
        //            "And I send an invalid verification code".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });
        //
        //            "And I send an invalid verification code a second time".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });
        //
        //            "And I send an invalid verification code a third time".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });
        //
        //            "Then I Request a new verification code".x(async () => { await this.driver.RequestSMSCode(); });
        //
        //            "Then I receive an SMS with the verification code in".x(async () => { await this.driver.WaitForSMS(); });
        //
        //            "And I can submit that code to verify my phone number".x(async () => { await this.driver.SubmitVerificationCode(); });
        //
        //            "Then my phone number is verified".x(() => { this.driver.CheckPhoneIsVerified(); });
        //        }

        //        [Fact]
        //        public Task Verifying_an_already_verified_phone_number()
        //        {
        //           return Task.CompletedTask;
        //        }

        private class Driver : IdentityTestDriver
        {
            private string smsCodeFromTwilio;

            private string invalidSmsCodeFromTwilio = "";

            private HttpResponseMessage submissionResponse;

            private HttpResponseMessage smsInvalidResponse;

            private HttpResponseMessage subsequentSmsResponse;

            private HttpResponseMessage unverifiedemailphoneResponse;

            public Driver(ApolloIntegrationFixture services) : base(services)
            {
                
            }

            public async Task RequestSMSCode()
            {
                var smsResponse = await this.Services.ApolloClient.PostAsync("/phone-verification",
                    new StringContent(JsonConvert.SerializeObject(new { phonenumber = "123" }), Encoding.UTF8, "application/json"));
                smsResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
            }

            public async Task RequestSubsequentSMSCode()
            {
                subsequentSmsResponse = await this.Services.ApolloClient.PostAsync("/phone-verification",
                    new StringContent(JsonConvert.SerializeObject(new { phonenumber = "123" }), Encoding.UTF8, "application/json"));
            }

            public Task WaitForSMS()
            {
                //todo note: because we don't have an easy way to receive SMS through Twilio, just grab it from TableStorage here and lean on other Twilio tests for integrations
                this.smsCodeFromTwilio = "todo";
                return Task.CompletedTask;
            }

            public async Task SubmitVerificationCode()
            {
                //todo get the code and include it in the request
                submissionResponse = await this.Services.ApolloClient.PostAsync("/phone-verification-submission",
                    new StringContent(JsonConvert.SerializeObject(new { code = smsCodeFromTwilio }), Encoding.UTF8, "application/json"));
            }

            public void CheckPhoneIsVerified()
            {
                //todo figure out how to check the status of claim
                this.submissionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            }

            public async Task SubmitInvalidVerificationCode()
            {
                submissionResponse = await this.Services.ApolloClient.PostAsync("/phone-verification-submission",
                    new StringContent(JsonConvert.SerializeObject(new { code = invalidSmsCodeFromTwilio }), Encoding.UTF8, "application/json"));
            }

            public async Task SubmitExpiredVerificationCode()
            {
                var submissionResponse = await this.Services.ApolloClient.PostAsync("/phone-verification-submission", null);
                submissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            public void CheckPhoneIsNotVerified()
            {
                //todo figure out how to check the status of claim
                this.submissionResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            }

            public async Task RequestSMSCodeWithUnverifiedEmail()
            {
                unverifiedemailphoneResponse = await this.Services.ApolloClient.PostAsync("/phone-verification",
                    new StringContent(JsonConvert.SerializeObject(new { phonenumber = "123" }), Encoding.UTF8, "application/json"));
            }

            public async Task RequestSMSCodeWithInvalidData()
            {
                smsInvalidResponse =
                    await this.Services.ApolloClient.PostAsync("/phone-verification", new StringContent(JsonConvert.SerializeObject(new { phonenumber = "" }), Encoding.UTF8, "application/json"));
            }

            public async Task CheckInvalidResponse()
            {
                this.smsInvalidResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            }

            public void CheckPhoneIsAlreadyBeingProcessed()
            {
                this.subsequentSmsResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            public async Task CheckPhoneIsNotVerifiedWithUnverifiedEmail()
            {
                this.unverifiedemailphoneResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }
    }
}
