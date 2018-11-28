// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Net.Sockets;

namespace Apollo.Tests.Unit.Sdk
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Npgsql;
    using Xunit;
    
    public class ApolloIntegrationFixture : IAsyncLifetime
    {
        private readonly PostgresContainer postgresContainer;
        private readonly IroncladContainer ironcladContainer;
        private readonly AzureStorageEmulatorContainer azureStorageEmulatorContainer;
        private readonly MailHogContainer mailhogContainer;

        public ApolloIntegrationFixture()
        {
            this.SmtpServerEndpoint = new Uri($"smtp://localhost:{PortManager.GetNextPort()}");
            this.SmtpServerHttpEndpoint = new Uri($"http://localhost:{PortManager.GetNextPort()}");
            var registryCredentials = new NetworkCredential(
                Environment.GetEnvironmentVariable("DOCKER_USERNAME"), 
                Environment.GetEnvironmentVariable("DOCKER_PASSWORD")
            );
            var googleCredentials = new NetworkCredential(
                Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID"), 
                Environment.GetEnvironmentVariable("GOOGLE_SECRET")
            );
            //var endpoint = new Uri($"http://localhost:{PortManager.GetNextPort()}");
            var endpoint = new Uri("http://localhost:5005");
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder($"Host={ResolveHost()};Database=ironclad;Username=postgres;Password=postgres;Port={PortManager.GetNextPort()}");
            this.postgresContainer = new PostgresContainer(connectionStringBuilder);
            this.ironcladContainer = new IroncladContainer(endpoint, connectionStringBuilder, registryCredentials, googleCredentials);
            this.azureStorageEmulatorContainer = new AzureStorageEmulatorContainer();
            this.mailhogContainer = new MailHogContainer(this.SmtpServerEndpoint, this.SmtpServerHttpEndpoint);
        }
        
        public Uri SmtpServerEndpoint { get; }
        public Uri SmtpServerHttpEndpoint { get; }
        
        // TODO: Expose all the relevant properties such that tests can connect to the various services 

        public virtual async Task InitializeAsync()
        {
            await Task.WhenAll(
                this.postgresContainer.InitializeAsync(),
                this.azureStorageEmulatorContainer.InitializeAsync(),
                this.mailhogContainer.InitializeAsync()
            ).ConfigureAwait(false);

            await this.ironcladContainer.InitializeAsync().ConfigureAwait(false);
        }

        public virtual async Task DisposeAsync()
        {
            await this.ironcladContainer.DisposeAsync().ConfigureAwait(false);
            
            await Task.WhenAll(
                this.postgresContainer.DisposeAsync(),
                this.azureStorageEmulatorContainer.DisposeAsync(),
                this.mailhogContainer.DisposeAsync()
            ).ConfigureAwait(false);
        }

        private static string ResolveHost()
        {
            if (Environment.OSVersion.Platform.Equals(PlatformID.Unix))
            {
                return Environment.MachineName;
            }
            
            //REMARK: Joe had an issue where name resolution was not working in Docker on Windows.
            //        This is a workaround.
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            //REMARK: Fallback instead of failure.
            return Environment.MachineName;
        }
    }
}