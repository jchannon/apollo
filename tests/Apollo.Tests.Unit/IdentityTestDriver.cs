// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Apollo.Tests.Unit.Sdk;
    using Bogus.DataSets;
    using FluentAssertions;
    using IdentityModel;
    using IdentityModel.Client;
    using Ironclad.Client;
    using Ironclad.Tests.Sdk;

    public abstract class IdentityTestDriver
    {
        protected IdentityTestDriver(ApolloIntegrationFixture services)
        {
            this.Services = services ?? throw new ArgumentNullException(nameof(services));

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
                PhoneNumber = phoneNumbers.PhoneNumber("+447#########")
            };
        }

        public ApolloIntegrationFixture Services { get; }

        public string UserId { get; set; }

        public User CurrentUser { get; set; }

        protected HttpClient ApolloClient { get; }

        public async Task RegisterUser(bool emailVerified = false, bool phoneVerified = false, string phoneNumber = default)
        {
            if (emailVerified)
            {
                this.CurrentUser.Claims.Add(JwtClaimTypes.EmailVerified, true);
            }

            if (phoneVerified)
            {
                this.CurrentUser.Claims.Add(JwtClaimTypes.PhoneNumberVerified, true);
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                this.CurrentUser.PhoneNumber = phoneNumber;
            }

            await this.Services.UsersClient.AddUserAsync(this.CurrentUser);
        }

        public async Task Login()
        {
            var url = new RequestUrl(this.Services.Authority + "/connect/authorize")
                .CreateAuthorizeUrl(
                    this.Services.Client.Id,
                    "id_token token",
                    "openid profile " + this.Services.ApiResource.Name,
                    this.Services.Client.RedirectUris.First(),
                    "state",
                    "nonce");

            var automation = new BrowserAutomation(this.CurrentUser.Username, this.CurrentUser.Password);
            await automation.NavigateToLoginAsync(url).ConfigureAwait(false);
            var authorizeResponse = await automation.LoginToAuthorizationServerAndCaptureRedirectAsync().ConfigureAwait(false);

            authorizeResponse.IsError.Should().BeFalse();
            this.Services.ApolloClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizeResponse.AccessToken);

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(authorizeResponse.AccessToken))
            {
                throw new InvalidOperationException("Unable to read JWT token after logging into Ironclad");
            }

            var token = handler.ReadJwtToken(authorizeResponse.AccessToken);

            this.UserId = token.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        }
    }
}
