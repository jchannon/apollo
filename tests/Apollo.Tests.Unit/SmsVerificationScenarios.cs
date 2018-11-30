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
using Xbehave;
using Xunit;

namespace Apollo.Tests.Unit
{
    [Collection(nameof(ApolloIntegrationCollection))]
    public class SmsVerificationScenarios
    {
        private readonly Driver driver;

        public SmsVerificationScenarios(ApolloIntegrationFixture services)
        {
            this.driver = new Driver(services);
        }

        [Scenario]
        public void Verifying_a_phone_number()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I receive an SMS with the verification code in".x(async () => { await this.driver.WaitForSMS(); });

            "And I can submit that code to verify my phone number".x(async () => { await this.driver.SubmitVerificationCode(); });

            "Then my phone number is verified".x(async () => { await this.driver.CheckPhoneIsVerified(); });
        }

        [Scenario]
        public void Providing_incorrect_code_three_times()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I receive an SMS with the verification code in".x(async () => { await this.driver.WaitForSMS(); });

            "And I send an invalid verification code".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });

            "And I send an invalid verification code a second time".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });

            "And I send an invalid verification code a third time".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });

            "And then I use the expired valid code".x(async () => { await this.driver.SubmitExpiredVerificationCode(); });

            "Then my phone number is not verified".x(async () => { await this.driver.CheckPhoneIsNotVerified(); });
        }

        [Scenario]
        public void Veryifying_phone_number_without_verified_email()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCodeWithUnverifiedEmail(); });

            "Then my phone number is not verified".x(async () => { await this.driver.CheckPhoneIsNotVerified(); });
        }

        [Scenario]
        public void Verifying_phone_number_without_requesting_a_code()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I send an invalid verification code without submitting one".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });

            "Then my phone number is not verified".x(async () => { await this.driver.CheckPhoneIsNotVerified(); });
        }

        [Scenario]
        public void Providing_incorrect_code_three_times_and_then_succeeding_with_new_code()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I receive an SMS with the verification code in".x(async () => { await this.driver.WaitForSMS(); });

            "And I send an invalid verification code".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });

            "And I send an invalid verification code a second time".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });

            "And I send an invalid verification code a third time".x(async () => { await this.driver.SubmitInvalidVerificationCode(); });

            "Then I Request a new verification code".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I receive an SMS with the verification code in".x(async () => { await this.driver.WaitForSMS(); });

            "And I can submit that code to verify my phone number".x(async () => { await this.driver.SubmitVerificationCode(); });

            "Then my phone number is verified".x(async () => { await this.driver.CheckPhoneIsVerified(); });
        }

        [Fact]
        public Task Verifying_an_already_verified_phone_number()
        {
            return Task.CompletedTask;
        }

        private class Driver
        {
            private readonly ApolloIntegrationFixture services;
            private readonly HttpClient ironcladClient;
            private readonly HttpClient apolloClient;
            private User currentUser;

            public Driver(ApolloIntegrationFixture services)
            {
                this.services = services ?? throw new ArgumentNullException(nameof(services));

                this.ironcladClient = new HttpClient(services.IdentityAuthorityAdminHandler)
                {
                    BaseAddress = services.IdentityAuthority
                };

                this.apolloClient = new HttpClient()
                {
                    BaseAddress = services.ApolloEndpoint
                };
            }

            public async Task RegisterUser()
            {
                currentUser = new User
                {
                    Username = Guid.NewGuid().ToString("N"),
                    Password = Guid.NewGuid().ToString("N"),
                    Email = $"{Guid.NewGuid():N}@example.com",
                    PhoneNumber = "123456789",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false
                };

                var response = await this.ironcladClient.PostAsync("/api/users",
                    new StringContent(JsonConvert.SerializeObject(currentUser), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
            }

            public async Task Login()
            {
                var url = new RequestUrl(this.services.IdentityAuthority + "connect/authorize")
                    .CreateAuthorizeUrl(ApolloIntegrationFixture.ApolloAuthClientId, "id_token token", $"openid profile {ApolloIntegrationFixture.ApolloAuthApiIdentifier}",
                        $"{this.services.ApolloEndpoint}/redirect", "state", "nonce");

                var automation = new BrowserAutomation("admin", "password");
                await automation.NavigateToLoginAsync(url).ConfigureAwait(false);
                var authorizeResponse = await automation.LoginToAuthorizationServerAndCaptureRedirectAsync().ConfigureAwait(false);

                // assert
                authorizeResponse.IsError.Should().BeFalse();
                authorizeResponse.AccessToken.Should().NotBeNullOrEmpty();

                this.apolloClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizeResponse.AccessToken);
            }

            public async Task RequestSMSCode()
            {
                var smsResponse = await this.apolloClient.PostAsync("/phone-verification", null);
                smsResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
            }

            public Task WaitForSMS()
            {
                //todo note: because we don't have an easy way to receive SMS through Twilio, just grab it from TableStorage here and lean on other Twilio tests for integrations
                throw new NotImplementedException();
            }

            public async Task SubmitVerificationCode()
            {
                //todo get the code and include it in the request
                var submissionResponse = await this.apolloClient.PostAsync("/phone-verification-submission", null);
                submissionResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
            }

            public Task CheckPhoneIsVerified()
            {
                //todo figure out how to check the status of claim
                throw new NotImplementedException();
            }

            public async Task SubmitInvalidVerificationCode()
            {
                var submissionResponse = await this.apolloClient.PostAsync("/phone-verification-submission", null);
                submissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            public async Task SubmitExpiredVerificationCode()
            {
                var submissionResponse = await this.apolloClient.PostAsync("/phone-verification-submission", null);
                submissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            public Task CheckPhoneIsNotVerified()
            {
                //todo figure out how to check the status of claim
                throw new NotImplementedException();
            }

            public async Task RequestSMSCodeWithUnverifiedEmail()
            {
                var smsResponse = await this.apolloClient.PostAsync("/phone-verification", null);
                smsResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public bool EmailConfirmed { get; set; }
            public string PhoneNumber { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
        }
    }
}