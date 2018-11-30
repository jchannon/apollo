using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ironclad.Tests.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Npgsql;
using Xunit;

namespace Apollo.Tests.Unit.Sdk
{
    public class IroncladComponent : IAsyncLifetime
    {
        private readonly string apiIdentifier;
        private readonly string clientId;
        private readonly Uri apolloEndpoint;
        private readonly PostgresContainer postgresContainer;
        private readonly IroncladContainer ironcladContainer;
        private readonly AuthenticationFixture authenticationFixture;

        public IroncladComponent(string apiIdentifier, string clientId, Uri apolloEndpoint)
        {
            this.apiIdentifier = apiIdentifier;
            this.clientId = clientId;
            this.apolloEndpoint = apolloEndpoint;
            var connectionStringBuilder =
                new NpgsqlConnectionStringBuilder($"Host={ResolveHost()};Database=ironclad;Username=postgres;Password=postgres;Port={PortManager.GetNextPort()}");

            var registryCredentials = new NetworkCredential(
                Environment.GetEnvironmentVariable("DOCKER_USERNAME"),
                Environment.GetEnvironmentVariable("DOCKER_PASSWORD")
            );
            var googleCredentials = new NetworkCredential(
                Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID"),
                Environment.GetEnvironmentVariable("GOOGLE_SECRET")
            );

            this.postgresContainer = new PostgresContainer(connectionStringBuilder);
            this.ironcladContainer = new IroncladContainer(Endpoint, connectionStringBuilder, registryCredentials, googleCredentials);
            this.authenticationFixture = new AuthenticationFixture();
        }

        public HttpMessageHandler Handler => this.authenticationFixture.Handler;
        public Uri Endpoint => new Uri("http://localhost:5005");

        public async Task InitializeAsync()
        {
            await this.postgresContainer.InitializeAsync().ConfigureAwait(false);

            await this.ironcladContainer.InitializeAsync().ConfigureAwait(false);

            await this.authenticationFixture.InitializeAsync().ConfigureAwait(false);

            await this.CreateClientAndApi().ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            await this.ironcladContainer.DisposeAsync().ConfigureAwait(false);

            await this.postgresContainer.DisposeAsync().ConfigureAwait(false);

            await this.authenticationFixture.DisposeAsync().ConfigureAwait(false);
        }

        private static string ResolveHost()
        {
            if (Environment.OSVersion.Platform.Equals(PlatformID.Unix) && !Environment.OSVersion.Platform.Equals(PlatformID.MacOSX))
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

        private async Task CreateClientAndApi()
        {
            var httpClient = new HttpClient(this.Handler)
            {
                BaseAddress = this.Endpoint
            };

            var apiResource = new
            {
                Name = this.apiIdentifier,
                ApiSecret = "secret",
                ApiScopes = new List<object>{ new 
                {
                    Name = this.apiIdentifier,
                    UserClaims = new List<string>{ "openid", "profile" }
                }},
                Enabled = true
            };
            
            await httpClient.PostAsync("/api/apiresources", new StringContent(JsonConvert.SerializeObject(apiResource, GetJsonSerializerSettings()), Encoding.UTF8, "application/json"));

            var client = new
            {
                Id = this.clientId,
                Name = this.clientId,
                AllowedCorsOrigins = new List<string> {this.apolloEndpoint.ToString()},
                RedirectUris = new List<string> {$"{this.apolloEndpoint}/redirect"},
                AllowedScopes = new List<string> {"openid", "profile", this.apiIdentifier},
                AllowAccessTokensViaBrowser = true,
                AllowedGrantTypes = new List<string> {"implicit"},
                RequireConsent = false,
                Enabled = true
            };

            await httpClient.PostAsync("/api/clients", new StringContent(JsonConvert.SerializeObject(client, GetJsonSerializerSettings()), Encoding.UTF8, "application/json"));
        }
        
        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }
    }
    
}