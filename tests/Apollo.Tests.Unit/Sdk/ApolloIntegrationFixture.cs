// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Net.Http;

namespace Apollo.Tests.Unit.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class ApolloIntegrationFixture : IAsyncLifetime
    {
        private readonly AzureStorageEmulatorContainer azureStorageEmulatorContainer;
        private readonly MailHogContainer mailhogContainer;
        private readonly IroncladComponent ironcladComponent;
        private readonly IAsyncLifetime apolloInstance;

        public ApolloIntegrationFixture()
        {
            this.SmtpServerEndpoint = new Uri($"smtp://localhost:{PortManager.GetNextPort()}");
            this.SmtpServerHttpEndpoint = new Uri($"http://localhost:{PortManager.GetNextPort()}");

            this.azureStorageEmulatorContainer = new AzureStorageEmulatorContainer();
            this.mailhogContainer = new MailHogContainer(this.SmtpServerEndpoint, this.SmtpServerHttpEndpoint);
            this.ironcladComponent = new IroncladComponent(ApolloAuthApiIdentifier, ApolloAuthClientId, ApolloEndpoint);
            this.apolloInstance = new BuiltFromSourceApollo(ApolloEndpoint);
        }

        public Uri SmtpServerEndpoint { get; }

        public Uri SmtpServerHttpEndpoint { get; }
        public Uri IdentityAuthority => this.ironcladComponent.Endpoint;

        public Uri ApolloEndpoint => new Uri("http://localhost:5006");

        public const string ApolloAuthApiIdentifier = "apollo";
        public const string ApolloAuthClientId = "apollo";


        public HttpMessageHandler IdentityAuthorityAdminHandler => this.ironcladComponent.Handler;
        // TODO: Expose all the relevant properties such that tests can connect to the various services 

        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                this.ironcladComponent.InitializeAsync(),
                this.azureStorageEmulatorContainer.InitializeAsync(),
                this.mailhogContainer.InitializeAsync()
            ).ConfigureAwait(false);

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