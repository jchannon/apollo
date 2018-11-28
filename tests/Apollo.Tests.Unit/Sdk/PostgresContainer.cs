// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Npgsql;

namespace Apollo.Tests.Unit.Sdk
{
    public class PostgresContainer : DockerContainer
    {
        public PostgresContainer(NpgsqlConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            
            this.Configuration = new DockerContainerConfiguration
            {
                Image = "postgres",
                Tag = "latest",
                AutoRemoveContainer = true,
                ContainerName = "apollo_tests_postgres",
                ContainerEnvironmentVariables = new []
                {
                    "POSTGRES_USER=" + connectionStringBuilder.Username,
                    "POSTGRES_PASSWORD=" + connectionStringBuilder.Password,
                    "POSTGRES_DB=" + connectionStringBuilder.Database
                },
                ContainerPortBindings = new []
                {
                    new DockerContainerPortBinding
                    {
                        GuestTcpPort = 5432, HostTcpPort = connectionStringBuilder.Port
                    }
                },
                MaximumWaitUntilAvailableAttempts = 5,
                TimeBetweenWaitUntilAvailableAttempts = TimeSpan.FromSeconds(1),
                WaitUntilAvailable = async token =>
                {
                    try
                    {
                        using (var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
                        {
                            await connection.OpenAsync().ConfigureAwait(false);
                        }

                        return true;
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