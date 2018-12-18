// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit.Sdk
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Apollo.Persistence;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class InMemoryApollo : IAsyncLifetime
    {
        private readonly string smtphost;

        private readonly string smtpport;

        private readonly string identityAuthority;

        private readonly string clientId;

        private readonly string secret;

        private TestServer testServer;

        private AzureInMemoryRepository azureInMemoryRepository;

        public InMemoryApollo(string smtphost, string smtpport, string identityAuthority, string clientId, string secret)
        {
            this.smtphost = smtphost;
            this.smtpport = smtpport;
            this.identityAuthority = identityAuthority;
            this.clientId = clientId;
            this.secret = secret;
        }

        public HttpClient HttpClient { get; private set; }

        public Task InitializeAsync()
        {
            Environment.SetEnvironmentVariable("SMTP__host", this.smtphost);
            Environment.SetEnvironmentVariable("SMTP__port", this.smtpport);
            Environment.SetEnvironmentVariable("IdentityServer__Authority", this.identityAuthority);
            Environment.SetEnvironmentVariable("IroncladClient__ClientId", this.clientId);
            Environment.SetEnvironmentVariable("IroncladClient__ClientSecret", this.secret);

            this.azureInMemoryRepository = new AzureInMemoryRepository();

            var builder = new WebHostBuilder()
                .ConfigureTestServices(services => services.AddSingleton<IVerificationRequestRepository>(this.azureInMemoryRepository))
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                    configurationBuilder
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile("appsettings.Custom.json", optional: true)
                        .AddEnvironmentVariables())
                .UseContentRoot(Path.GetDirectoryName(typeof(Startup).Assembly.Location));

            this.testServer = new TestServer(builder);

            this.HttpClient = this.testServer.CreateClient();

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            this.testServer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
