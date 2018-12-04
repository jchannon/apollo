namespace Apollo.Tests.Unit
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Apollo.Tests.Unit.Sdk;
    using FluentAssertions;
    using IdentityModel.Client;
    using Ironclad.Tests.Sdk;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
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
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number with invalid data".x(async () => { await this.driver.RequestSMSCodeWithInvalidData(); });

            "Then I get a API response to indication validation failure".x(async () => { await this.driver.CheckInvalidResponse(); });
        }

        [Scenario]
        public void Verifying_a_phone_number()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I receive an SMS with the verification code in".x(async () => { await this.driver.WaitForSMS(); });

            "And I can submit that code to verify my phone number".x(async () => { await this.driver.SubmitVerificationCode(); });

            "Then my phone number is verified".x(() => { this.driver.CheckPhoneIsVerified(); });
        }

        [Scenario]
        public void Verifying_a_phone_number_when_already_asked_for_verification()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I request a code to be verified again".x(async () => { await this.driver.RequestSubsequentSMSCode(); });

            "Then the response shows a bad request".x(() => { this.driver.CheckPhoneIsAlreadyBeingProcessed(); });
        }

        [Scenario]
        public void Verifying_phone_number_without_requesting_a_code()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I send an invalid verification code without submitting one".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });

            "Then my phone number is not verified".x(() => { this.driver.CheckPhoneIsNotVerified(); });
        }

        [Scenario]
        public void Veryifying_phone_number_without_verified_email()
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(emailConfirmed: false); });

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

        private class Driver
        {
            private readonly ApolloIntegrationFixture services;

            private readonly HttpClient ironcladClient;

            private readonly HttpClient apolloClient;

            private User currentUser;

            private string smsCodeFromTwilio;

            private string invalidSmsCodeFromTwilio = "";

            private HttpResponseMessage submissionResponse;

            private HttpResponseMessage smsInvalidResponse;

            private HttpResponseMessage subsequentSmsResponse;

            private HttpResponseMessage unverifiedemailphoneResponse;

            public Driver(ApolloIntegrationFixture services)
            {
                this.services = services ?? throw new ArgumentNullException(nameof(services));

                this.ironcladClient = new HttpClient(services.IdentityAuthorityAdminHandler)
                {
                    BaseAddress = services.IdentityAuthority
                };

                this.apolloClient = new HttpClient
                {
                    BaseAddress = services.ApolloEndpoint
                };
            }

            public async Task RegisterUser(bool emailConfirmed = true)
            {
                currentUser = new User
                {
                    Username = Guid.NewGuid().ToString("N"),
                    Password = Guid.NewGuid().ToString("N"),
                    Email = $"{Guid.NewGuid():N}@example.com",
                    PhoneNumber = "07974666916",
                    EmailVerified = emailConfirmed,
                    PhoneNumberVerified = false,
                };

                var response = await this.ironcladClient.PostAsync("/api/users",
                    new StringContent(JsonConvert.SerializeObject(currentUser, GetJsonSerializerSettings()), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
            }

            public async Task Login()
            {
                var url = new RequestUrl(this.services.IdentityAuthority + "connect/authorize")
                    .CreateAuthorizeUrl(ApolloIntegrationFixture.ApolloAuthClientId, "id_token token", $"openid profile email phone {ApolloIntegrationFixture.ApolloAuthApiIdentifier}",
                        $"{this.services.ApolloEndpoint}/redirect", "state", "nonce");

                var automation = new BrowserAutomation(currentUser.Username, currentUser.Password);
                await automation.NavigateToLoginAsync(url).ConfigureAwait(false);
                var authorizeResponse = await automation.LoginToAuthorizationServerAndCaptureRedirectAsync().ConfigureAwait(false);

                // assert
                authorizeResponse.IsError.Should().BeFalse();
                authorizeResponse.AccessToken.Should().NotBeNullOrEmpty();

                this.apolloClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizeResponse.AccessToken);
            }

            public async Task RequestSMSCode()
            {
                var smsResponse = await this.apolloClient.PostAsync("/phone-verification",
                    new StringContent(JsonConvert.SerializeObject(new { phonenumber = "123" }), Encoding.UTF8, "application/json"));
                smsResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
            }

            public async Task RequestSubsequentSMSCode()
            {
                subsequentSmsResponse = await this.apolloClient.PostAsync("/phone-verification",
                    new StringContent(JsonConvert.SerializeObject(new { phonenumber = "123" }), Encoding.UTF8, "application/json"));
            }

            public Task WaitForSMS()
            {
                //todo note: because we don't have an easy way to receive SMS through Twilio, just grab it from TableStorage here and lean on other Twilio tests for integrations
                smsCodeFromTwilio = "todo";
                return Task.CompletedTask;
            }

            public async Task SubmitVerificationCode()
            {
                //todo get the code and include it in the request
                submissionResponse = await this.apolloClient.PostAsync("/phone-verification-submission",
                    new StringContent(JsonConvert.SerializeObject(new { code = smsCodeFromTwilio }), Encoding.UTF8, "application/json"));
            }

            public void CheckPhoneIsVerified()
            {
                //todo figure out how to check the status of claim
                submissionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            }

            public async Task SubmitInvalidVerificationCode()
            {
                submissionResponse = await this.apolloClient.PostAsync("/phone-verification-submission",
                    new StringContent(JsonConvert.SerializeObject(new { code = invalidSmsCodeFromTwilio }), Encoding.UTF8, "application/json"));
            }

            public async Task SubmitExpiredVerificationCode()
            {
                var submissionResponse = await this.apolloClient.PostAsync("/phone-verification-submission", null);
                submissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            public void CheckPhoneIsNotVerified()
            {
                //todo figure out how to check the status of claim
                submissionResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            }

            public async Task RequestSMSCodeWithUnverifiedEmail()
            {
                unverifiedemailphoneResponse = await this.apolloClient.PostAsync("/phone-verification",
                    new StringContent(JsonConvert.SerializeObject(new { phonenumber = "123" }), Encoding.UTF8, "application/json"));
            }

            public async Task RequestSMSCodeWithInvalidData()
            {
                smsInvalidResponse =
                    await this.apolloClient.PostAsync("/phone-verification", new StringContent(JsonConvert.SerializeObject(new { phonenumber = "" }), Encoding.UTF8, "application/json"));
            }

            public async Task CheckInvalidResponse()
            {
                smsInvalidResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            }

            public void CheckPhoneIsAlreadyBeingProcessed()
            {
                subsequentSmsResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            public async Task CheckPhoneIsNotVerifiedWithUnverifiedEmail()
            {
                unverifiedemailphoneResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            private static JsonSerializerSettings GetJsonSerializerSettings()
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                    NullValueHandling = NullValueHandling.Ignore,
                };

                settings.Converters.Add(new StringEnumConverter());

                return settings;
            }
        }
    }
}
