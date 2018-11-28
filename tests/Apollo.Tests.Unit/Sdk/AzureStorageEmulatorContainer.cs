// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.WindowsAzure.Storage;

namespace Apollo.Tests.Unit.Sdk
{
    public class AzureStorageEmulatorContainer : DockerContainer
    {
        public AzureStorageEmulatorContainer()
        {
            const string ip = "127.0.0.1";
            
            var blobport = PortManager.GetNextPort();
            var queueport = PortManager.GetNextPort();
            var tableport = PortManager.GetNextPort();

            this.Account = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;"
                + "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;"
                + $"BlobEndpoint=http://{ip}:{blobport}/devstoreaccount1;"
                + $"TableEndpoint=http://{ip}:{tableport}/devstoreaccount1;"
                + $"QueueEndpoint=http://{ip}:{queueport}/devstoreaccount1;"
            );
            
            this.Configuration = new DockerContainerConfiguration
            {
                //REMARK: the microsoft docker image is windows based which does not run on linux
                Image = "arafato/azurite",
                Tag = "latest",
                AutoRemoveContainer = true,
                ContainerName = "apollo_tests_azure",
                ContainerEnvironmentVariables = new string[0],
                ContainerPortBindings = new []
                {
                    new DockerContainerPortBinding
                    {
                        GuestTcpPort = 10000, HostTcpPort = blobport
                    },
                    new DockerContainerPortBinding
                    {
                        GuestTcpPort = 10001, HostTcpPort = queueport
                    },
                    new DockerContainerPortBinding
                    {
                        GuestTcpPort = 10002, HostTcpPort = tableport
                    }
                },
                MaximumWaitUntilAvailableAttempts = 5,
                TimeBetweenWaitUntilAvailableAttempts = TimeSpan.FromSeconds(1),
                WaitUntilAvailable = async token =>
                {
                    try
                    {
                        var client = this.Account.CreateCloudBlobClient();
                        await client.GetRootContainerReference().CreateIfNotExistsAsync();

                        return true;
                    }
                    catch
                    {
                    }

                    return false;
                }
            };
        }

        public CloudStorageAccount Account { get; }
    }
}