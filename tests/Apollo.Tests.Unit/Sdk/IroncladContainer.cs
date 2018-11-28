// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Net.Http;
using Npgsql;

namespace Apollo.Tests.Unit.Sdk
{
    public class IroncladContainer : DockerContainer
    {
        public IroncladContainer(Uri endpoint, NpgsqlConnectionStringBuilder connectionStringBuilder, NetworkCredential registryCredentials, NetworkCredential googleCredentials)
        {
            if (endpoint == null) 
                throw new ArgumentNullException(nameof(endpoint));
            if (connectionStringBuilder == null) 
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            if (registryCredentials == null) 
                throw new ArgumentNullException(nameof(registryCredentials));

            var endpointBuilder = new UriBuilder(endpoint);
            endpointBuilder.Path = endpointBuilder.Path.EndsWith("/")
                ? endpointBuilder.Path + "api"
                : endpointBuilder.Path + "/api";

            this.Configuration = new DockerContainerConfiguration
            {
                Registry = "lykkecloud.azurecr.io",
                RegistryCredentials = registryCredentials,
                Image = "ironclad",
                Tag = "latest",
                AutoRemoveContainer = true,
                ContainerName = "apollo_tests_ironclad",
                ContainerEnvironmentVariables = new[]
                {
                    $"IRONCLAD_CONNECTIONSTRING={connectionStringBuilder.ConnectionString}",
                    $"GOOGLE_CLIENT_ID={googleCredentials.UserName}",
                    $"GOOGLE_SECRET={googleCredentials.Password}"
                },
                ContainerPortBindings = new[]
                {
                    new DockerContainerPortBinding
                    {
                        GuestTcpPort = 80, HostTcpPort = endpoint.Port
                    }
                },
                WaitUntilAvailable = async token =>
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            using (var response = await client.GetAsync(endpointBuilder.Uri, token).ConfigureAwait(false))
                            {
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    return true;
                                }
                            }
                        }
                        catch (HttpRequestException)
                        {
                        }
                    }

                    return false;
                },
                MaximumWaitUntilAvailableAttempts = 15,
                TimeBetweenWaitUntilAvailableAttempts = TimeSpan.FromSeconds(1)
            };
        }
    }
}