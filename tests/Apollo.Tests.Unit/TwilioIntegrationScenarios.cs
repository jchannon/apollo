namespace Apollo.Tests.Unit
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using IdentityModel.Client;
    using Ironclad.Tests.Sdk;
    using Newtonsoft.Json;
    using Apollo.Tests.Unit.Sdk;
    using Xbehave;
    using Xunit;

    [Collection(nameof(ApolloIntegrationCollection))]
    public class TwilioIntegrationScenarios
    {
        private readonly Driver driver;

        public TwilioIntegrationScenarios(ApolloIntegrationFixture services)
        {
            this.driver = new Driver(services);
        }

        [Scenario]
        public void CanSendSMSToValidNumber()
        {
            "Given I have a user with a verified email and a valid unverified phone number".x(async () => { await this.driver.RegisterUserWithNumber("+15005550123"); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I Request an SMS Code".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I get a valid response code".x(() => { this.driver.ResponseIsValid(); });
        }

        [Scenario]
        public void CannotSendSMSToInvalidNumber()
        {
            "Given I have a user with a verified email and an invalid unverified phone number".x(async () => { await this.driver.RegisterUserWithNumber("+15005550001"); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I Request an SMS Code".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I get a invalid response code".x(() => { this.driver.ResponseIsInvalid(); });
        }

        [Scenario]
        public void CannotSendSMSToBlacklistedNumber()
        {
            "Given I have a user with a verified email and a blacklisted, unverified phone number".x(async () =>
            {
                await this.driver.RegisterUserWithNumber("+15005550004");
            });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I Request an SMS Code".x(async () => { await this.driver.RequestSMSCode(); });

            "Then I get a invalid response code".x(() => { this.driver.ResponseIsInvalid(); });
        }

        [Scenario]
        public void CannotSendSMSToNumberIncapableOfReceivingSMS()
        {
            "Given I have a user with a verified email and a unverified phone number which cannot receive SMS".x(async () =>
            {
                await this.driver.RegisterUserWithNumber("+15005550009");
            });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I Request an SMS Code".x(async () => {await this.driver.RequestSMSCode(); });

            "Then I get a invalid response code".x(() => { this.driver.ResponseIsInvalid(); });
        }

        private sealed class Driver
        {
            private readonly ApolloIntegrationFixture services;

            private readonly HttpClient ironcladClient;

            private readonly HttpClient apolloClient;

            private User currentUser;

            private HttpResponseMessage smsCodeResponse;

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

            public async Task RegisterUserWithNumber(string phoneNumber)
            {
                currentUser = new User
                {
                    Username = Guid.NewGuid().ToString("N"),
                    Password = Guid.NewGuid().ToString("N"),
                    Email = $"{Guid.NewGuid():N}@example.com",
                    PhoneNumber = phoneNumber,
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

                var automation = new BrowserAutomation(this.currentUser.Username, this.currentUser.Password);
                await automation.NavigateToLoginAsync(url).ConfigureAwait(false);
                var authorizeResponse = await automation.LoginToAuthorizationServerAndCaptureRedirectAsync().ConfigureAwait(false);

                // assert
                authorizeResponse.IsError.Should().BeFalse();
                authorizeResponse.AccessToken.Should().NotBeNullOrEmpty();

                this.apolloClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizeResponse.AccessToken);
            }

            public async Task RequestSMSCode()
            {
                this.smsCodeResponse = await this.apolloClient.PostAsync("/phone-verification", null);
            }

            public void ResponseIsValid()
            {
                this.smsCodeResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
            }

            public void ResponseIsInvalid()
            {
                this.smsCodeResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
                //todo body verifcation
            }
        }
    }
}
