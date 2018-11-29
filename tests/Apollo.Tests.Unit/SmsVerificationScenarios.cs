using System;
using System.Net.Http;
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
        private Driver driver;

        public SmsVerificationScenarios(ApolloIntegrationFixture services)
        {
            this.driver = new Driver(services);
        }

        [Scenario]
        public void Verifying_a_phone_number()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "I can login as the user".x(async () => { await this.driver.Login(); });

//            await this.driver.SendRequestToVerifyPhoneNumberBySMS();
//
//            var code = await this.driver.WaitForSMSWithCodeToArrive();
//
//            await this.driver.SendSMSConfirmationCode(code);
//
//            await this.driver.WaitForPhoneVerifiedToBeSetInIronclad();
        }

        [Fact]
        public Task Providing_incorrect_code_three_times()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyPhoneNumberBySMS();
//
//            var code = await this.driver.WaitForSMSWithCodeToArrive();
//
//            await this.driver.SendInvalidConfirmationCode(code);
//
//            await this.driver.SendInvalidConfirmationCode(code);
//
//            await this.driver.SendInvalidConfirmationCode(code);
//
//            await this.driver.UseExpiredVerificationCode();

            return Task.CompletedTask;
        }

        [Fact]
        public Task Veryifying_phone_number_without_verified_email()
        {
//            await this.driver.RegisterUserWithUnverifiedEmail();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyPhoneNumberBySMSAndBeRejected();

            return Task.CompletedTask;
        }

        [Fact]
        public Task Verifying_phone_number_without_requesting_a_code()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendInvalidConfirmationCode(code);
//

            return Task.CompletedTask;
        }

        [Fact]
        public Task Providing_incorrect_code_three_times_and_then_succeeding_with_new_code()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyPhoneNumberBySMS();
//
//            var code = await this.driver.WaitForSMSWithCodeToArrive();
//
//            await this.driver.SendInvalidConfirmationCode(code);
//
//            await this.driver.SendInvalidConfirmationCode(code);
//
//            await this.driver.SendInvalidConfirmationCode(code);
//
//            await this.driver.RequestNewVerificationCode(code);
//
//            var code = await this.driver.WaitForSMSWithCodeToArrive();
//
//            await this.driver.SendSMSConfirmationCode(code);
//
//            await this.driver.WaitForPhoneVerifiedToBeSetInIronclad();

            return Task.CompletedTask;
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
            private User currentUser;
            private string accessToken;

            public Driver(ApolloIntegrationFixture services)
            {
                this.services = services ?? throw new ArgumentNullException(nameof(services));

                this.ironcladClient = new HttpClient(services.IdentityAuthorityAdminHandler)
                {
                    BaseAddress = services.IdentityAuthority
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
                accessToken = authorizeResponse.AccessToken;
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