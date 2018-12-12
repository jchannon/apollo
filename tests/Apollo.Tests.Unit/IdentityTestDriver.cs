namespace Apollo.Tests.Unit
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Linq;
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
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public abstract class IdentityTestDriver
    {
        private readonly HttpClient ironcladClient;

        protected ApolloIntegrationFixture Services { get; }

        public User CurrentUser { get; set; }

        protected IdentityTestDriver(ApolloIntegrationFixture services)
        {
            this.Services = services ?? throw new ArgumentNullException(nameof(services));

            this.ironcladClient = new HttpClient(services.IdentityAuthorityAdminHandler)
            {
                BaseAddress = services.IdentityAuthority
            };

            var internet = new Internet();
            var phoneNumbers = new PhoneNumbers();

            var email = internet.Email();

            this.CurrentUser = new User
            {
                Username = email,
                Password = internet.Password(),
                Email = email,
                PhoneNumber = phoneNumbers.PhoneNumber("+447#########")
            };
        }

        public async Task RegisterUser(bool emailVerified = false, bool phoneVerified = false, string phoneNumber = null)
        {
            if (emailVerified)
            {
                this.CurrentUser.VerifyEmail();
            }

            if (phoneVerified)
            {
                this.CurrentUser.VerifyPhone();
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                this.CurrentUser.PhoneNumber = phoneNumber;
            }

            var response = await this.ironcladClient.PostAsync("/api/users",
                new StringContent(JsonConvert.SerializeObject(this.CurrentUser, GetJsonSerializerSettings()), Encoding.UTF8, "application/json"));

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
            this.Services.ApolloClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizeResponse.AccessToken);

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(authorizeResponse.AccessToken))
            {
                throw new InvalidOperationException("Unable to read JWT token after logging into Ironclad");
            }

            var token = handler.ReadJwtToken(authorizeResponse.AccessToken);

            this.CurrentUser.UserId = token.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }
    }
}
