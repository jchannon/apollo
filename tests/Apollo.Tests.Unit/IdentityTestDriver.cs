namespace Apollo.Tests.Unit
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Apollo.Tests.Unit.Sdk;
    using Bogus.DataSets;
    using FluentAssertions;
    using IdentityModel.Client;
    using Ironclad.Tests.Sdk;
    using Newtonsoft.Json;

    public abstract class IdentityTestDriver
    {
        private readonly HttpClient ironcladClient;

        protected IdentityTestDriver(ApolloIntegrationFixture services)
        {
            this.Services = services ?? throw new ArgumentNullException(nameof(services));

            this.ironcladClient = new HttpClient(services.IdentityAuthorityAdminHandler)
            {
                BaseAddress = services.IdentityAuthority
            };

            this.ApolloClient = new HttpClient
            {
                BaseAddress = services.ApolloEndpoint
            };

            var internet = new Internet();
            var phoneNumbers = new PhoneNumbers();

            var email = internet.Email();

            this.CurrentUser = new User
            {
                Username = email,
                Password = internet.Password(),
                Email = email,
                PhoneNumber = phoneNumbers.PhoneNumber()
            };
        }
        
        public User CurrentUser { get; set; }

        protected ApolloIntegrationFixture Services { get; }

        protected HttpClient ApolloClient { get; }


        public async Task RegisterUser(bool emailVerified = false, bool phoneVerified = false)
        {
            if (emailVerified)
            {
                this.CurrentUser.VerifyEmail();
            }

            if (phoneVerified)
            {
                this.CurrentUser.VerifyPhone();
            }

            var response = await this.ironcladClient.PostAsync("/api/users",
                new StringContent(JsonConvert.SerializeObject(this.CurrentUser), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }

        public async Task Login()
        {
            var url = new RequestUrl(this.Services.IdentityAuthority + "connect/authorize")
                .CreateAuthorizeUrl(ApolloIntegrationFixture.ApolloAuthClientId, "id_token token", $"openid profile {ApolloIntegrationFixture.ApolloAuthApiIdentifier}",
                    $"{this.Services.ApolloEndpoint}/redirect", "state", "nonce");

            var automation = new BrowserAutomation(this.CurrentUser.Username, this.CurrentUser.Password);
            await automation.NavigateToLoginAsync(url).ConfigureAwait(false);
            var authorizeResponse = await automation.LoginToAuthorizationServerAndCaptureRedirectAsync().ConfigureAwait(false);

            // assert
            authorizeResponse.IsError.Should().BeFalse();
            this.ApolloClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizeResponse.AccessToken);
        }
    }
}
