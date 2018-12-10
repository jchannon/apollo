// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit.Sdk
{
    using System;
    using System.Net.Http;
    using System.IO;
    using System.Threading.Tasks;
    using Apollo.Persistence;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class ApolloIntegrationFixture : IAsyncLifetime
    {
        public const string ApolloAuthApiIdentifier = "apollo";

        public const string ApolloAuthClientId = "apollo";

        public const string ApolloEndpointUri = "http://localhost:5006/status";

        private readonly IroncladComponent ironcladComponent;

        private readonly IAsyncLifetime apolloInstance;

        private readonly MailHogContainer mailhogContainer;

        private readonly AzureInMemoryRepository azureInMemoryRepository;

        private readonly TestServer testServer;

        public ApolloIntegrationFixture()
        {
            this.SmtpServerEndpoint = new Uri($"smtp://localhost:{PortManager.GetNextPort()}");
            this.SmtpServerHttpEndpoint = new Uri($"http://localhost:{PortManager.GetNextPort()}");

            this.mailhogContainer = new MailHogContainer(this.SmtpServerEndpoint, this.SmtpServerHttpEndpoint);
            this.ironcladComponent = new IroncladComponent(ApolloAuthApiIdentifier, ApolloAuthClientId, ApolloEndpoint);

            if (Environment.GetEnvironmentVariable("useinmemoryapollo") != null)
            {
                this.apolloInstance = new BuiltFromSourceApollo(this.ApolloEndpoint, this.SmtpServerEndpoint);
                this.ApolloClient = new HttpClient
                {
                    BaseAddress = this.ApolloEndpoint
                };
            }
            else
            {
                this.apolloInstance = new InMemoryApollo(this.SmtpServerEndpoint.Host, this.SmtpServerEndpoint.Port.ToString(), this.IdentityAuthority.ToString());
            }
        }

        public Uri SmtpServerEndpoint { get; }

        public Uri SmtpServerHttpEndpoint { get; }

        public Uri IdentityAuthority => this.ironcladComponent.Endpoint;

        public Uri ApolloEndpoint => new Uri(ApolloEndpointUri);

        public HttpMessageHandler IdentityAuthorityAdminHandler => this.ironcladComponent.Handler;

        public HttpClient ApolloClient { get; set; }
        // TODO: Expose all the relevant properties such that tests can connect to the various services 

        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                this.ironcladComponent.InitializeAsync(),
                this.mailhogContainer.InitializeAsync()
            ).ConfigureAwait(false);

            await this.apolloInstance.InitializeAsync();

            if (Environment.GetEnvironmentVariable("useinmemoryapollo") == null)
            {
                this.ApolloClient = ((InMemoryApollo)this.apolloInstance).HttpClient;
            }
        }

        public async Task DisposeAsync()
        {
            await this.apolloInstance.DisposeAsync();

            await Task.WhenAll(
                this.ironcladComponent.DisposeAsync(),
                this.mailhogContainer.DisposeAsync()
            ).ConfigureAwait(false);
        }
    }
}
