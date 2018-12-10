// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit.Sdk
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class ApolloIntegrationFixture : IAsyncLifetime
    {
        public const string ApolloAuthApiIdentifier = "apollo";

        public const string ApolloAuthClientId = "apollo";

        public const string ApolloEndpointUri = "http://localhost:5006/status";

        private readonly IAsyncLifetime apolloInstance;

        private readonly AzureStorageEmulatorContainer azureStorageEmulatorContainer;

        private readonly IroncladComponent ironcladComponent;

        private readonly MailHogContainer mailhogContainer;

        public ApolloIntegrationFixture()
        {
            this.SmtpServerEndpoint = new Uri($"smtp://localhost:{PortManager.GetNextPort()}");
            this.SmtpServerHttpEndpoint = new Uri($"http://localhost:{PortManager.GetNextPort()}");

            this.azureStorageEmulatorContainer = new AzureStorageEmulatorContainer();
            this.mailhogContainer = new MailHogContainer(this.SmtpServerEndpoint, this.SmtpServerHttpEndpoint);
            this.ironcladComponent = new IroncladComponent(ApolloAuthApiIdentifier, ApolloAuthClientId, this.ApolloEndpoint);
            this.apolloInstance = new BuiltFromSourceApollo(this.ApolloEndpoint, this.SmtpServerEndpoint);
        }

        public Uri SmtpServerEndpoint { get; }

        public Uri SmtpServerHttpEndpoint { get; }

        public Uri IdentityAuthority => this.ironcladComponent.Endpoint;

        public Uri ApolloEndpoint => new Uri(ApolloEndpointUri);

        public HttpMessageHandler IdentityAuthorityAdminHandler => this.ironcladComponent.Handler;
        // TODO: Expose all the relevant properties such that tests can connect to the various services 

        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                this.ironcladComponent.InitializeAsync(),
                this.azureStorageEmulatorContainer.InitializeAsync(),
                this.mailhogContainer.InitializeAsync()).ConfigureAwait(false);

            await this.apolloInstance.InitializeAsync().ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            await this.apolloInstance.DisposeAsync().ConfigureAwait(false);

            await Task.WhenAll(
                this.ironcladComponent.DisposeAsync(),
                this.azureStorageEmulatorContainer.DisposeAsync(),
                this.mailhogContainer.DisposeAsync()
            ).ConfigureAwait(false);
        }
    }
}
