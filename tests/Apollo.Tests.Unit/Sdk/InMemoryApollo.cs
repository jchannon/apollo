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

        private TestServer testServer;

        private AzureInMemoryRepository azureInMemoryRepository;

        public HttpClient HttpClient { get; private set; }

        public InMemoryApollo(string smtphost, string smtpport, string identityAuthority)
        {
            this.smtphost = smtphost;
            this.smtpport = smtpport;
            this.identityAuthority = identityAuthority;
        }

        public Task InitializeAsync()
        {
            Environment.SetEnvironmentVariable("SMTP__host", this.smtphost);
            Environment.SetEnvironmentVariable("SMTP__port", this.smtpport);
            Environment.SetEnvironmentVariable("IdentityServer__Authority", this.identityAuthority);

            this.azureInMemoryRepository = new AzureInMemoryRepository();

            var builder = new WebHostBuilder()
                .ConfigureTestServices(services => services.AddSingleton<IVerificationRequestRepository>(this.azureInMemoryRepository))
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddJsonFile("appsettings.json").AddEnvironmentVariables())
                .UseContentRoot(Path.GetDirectoryName(typeof(Startup).Assembly.Location));

            this.testServer = new TestServer(builder);

            this.HttpClient = this.testServer.CreateClient();
            
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            this.testServer.Dispose();
            return Task.CompletedTask;
        }
    }
}
