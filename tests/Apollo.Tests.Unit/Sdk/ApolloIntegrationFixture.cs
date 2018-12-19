// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit.Sdk
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using Ironclad.Tests.Sdk;
    using Xunit;

    public class ApolloIntegrationFixture : AuthenticationFixture
    {
        public const string ApolloEndpointUri = "http://localhost:5006/status";

        private readonly MailHogContainer mailhogContainer;

        private readonly IAsyncLifetime apolloInstance;

        public ApolloIntegrationFixture()
        {
            this.SmtpServerEndpoint = new Uri($"smtp://localhost:{PortManager.GetNextPort()}");

            var secret = "apolloclient";
            var clientId = "apollo_client";

            this.IroncladClientForApollo = new Client
            {
                Id = clientId,
                Name = "Apollo Client",
                AllowedScopes = { "auth_api", "auth_api:write" },
                AllowedGrantTypes = { "client_credentials" },
                Enabled = true,
                Secret = secret
            };

            this.SmtpServerHttpEndpoint = new Uri($"http://localhost:{PortManager.GetNextPort()}");

            this.mailhogContainer = new MailHogContainer(this.SmtpServerEndpoint, this.SmtpServerHttpEndpoint);

            if (Environment.GetEnvironmentVariable("useinmemoryapollo") != null)
            {
                this.apolloInstance = new BuiltFromSourceApollo(this.ApolloEndpoint, this.SmtpServerEndpoint, this.Authority);
                this.ApolloClient = new HttpClient
                {
                    BaseAddress = this.ApolloEndpoint
                };
            }
            else
            {
                this.apolloInstance = new InMemoryApollo(this.SmtpServerEndpoint.Host, this.SmtpServerEndpoint.Port.ToString(), this.Authority, clientId, secret);
            }
        }

        public ApiResource ApiResource { get; } = new ApiResource
        {
            Name = "apollo_api",
            ApiSecret = "secret",
            DisplayName = "Apollo API",
            UserClaims = { "phone_number", "phone_number_verified", "email", "email_verified" },

            // NOTE (Cameron): OMG wat?
            // LINK (Cameron): https://github.com/IdentityServer/IdentityServer4/blob/2.1.1/src/IdentityServer4/Models/ApiResource.cs#L67
            ApiScopes = { new ApiResource.Scope { Name = "apollo_api", UserClaims = { "phone_number", "phone_number_verified", "email", "email_verified" } } },

            Enabled = true
        };

        public Client Client { get; } = new Client
        {
            Id = "apollo_spa",
            Name = $"Apollo Single Page Application",
            AllowedCorsOrigins = { "http://localhost:5006" },
            RedirectUris = { "http://localhost:5006/redirect" },
            AllowedScopes = { "openid", "profile", "apollo_api" },
            AllowAccessTokensViaBrowser = true,
            AllowedGrantTypes = { "implicit" },
            RequireConsent = false,
            Enabled = true,
            AccessTokenType = "Reference"
        };

        public Client IroncladClientForApollo { get; }

        public Uri SmtpServerEndpoint { get; }

        public Uri SmtpServerHttpEndpoint { get; }

        public Uri ApolloEndpoint => new Uri(ApolloEndpointUri);

        public HttpClient ApolloClient { get; set; }

        protected override async Task OnInitializeAsync()
        {
            await base.OnInitializeAsync();

            await this.ApiResourcesClient.AddApiResourceAsync(this.ApiResource);
            await this.ClientsClient.AddClientAsync(this.Client);
            await this.ClientsClient.AddClientAsync(this.IroncladClientForApollo);

            await this.mailhogContainer.InitializeAsync().ConfigureAwait(false);
            await this.apolloInstance.InitializeAsync().ConfigureAwait(false);

            if (Environment.GetEnvironmentVariable("useinmemoryapollo") == null)
            {
                this.ApolloClient = ((InMemoryApollo)this.apolloInstance).HttpClient;
            }
        }

        protected override async Task OnDisposeAsync()
        {
            await this.apolloInstance.DisposeAsync().ConfigureAwait(false);
            await this.mailhogContainer.DisposeAsync().ConfigureAwait(false);

            await base.OnDisposeAsync();
        }
    }
}
