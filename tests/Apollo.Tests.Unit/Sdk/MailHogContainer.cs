//// Copyright (c) Lykke Corp.
//// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using Npgsql;

namespace Apollo.Tests.Unit.Sdk
{
    public class MailHogContainer : DockerContainer
    {
        public MailHogContainer(Uri smtpEndpoint, Uri httpEndpoint)
        {
            this.Configuration = new DockerContainerConfiguration
            {
                Image = "mailhog/mailhog",
                Tag = "latest",
                AutoRemoveContainer = true,
                ContainerName = "apollo_tests_mailhog",
                ContainerEnvironmentVariables = new string[0],
                ContainerPortBindings = new []
                {
                    new DockerContainerPortBinding
                    {
                        GuestTcpPort = 1025, HostTcpPort = smtpEndpoint.Port
                    },
                    new DockerContainerPortBinding
                    {
                        GuestTcpPort = 8025, HostTcpPort = httpEndpoint.Port
                    }
                },
                MaximumWaitUntilAvailableAttempts = 5,
                TimeBetweenWaitUntilAvailableAttempts = TimeSpan.FromSeconds(1),
                WaitUntilAvailable = async token =>
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var requestUri = new UriBuilder(httpEndpoint) {Path = "/api/v1/messages"}.Uri;
                            using (var response = await client.GetAsync(requestUri, token))
                            {
                                if (response.IsSuccessStatusCode) return true;
                            }
                        }

                        return false;
                    }
                    catch
                    {
                        
                    }

                    return false;
                }
            };
        }
    }
}