// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable CA1819

namespace Apollo.Tests.Unit.Sdk
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public class DockerContainerConfiguration
    {
        // Image related
        public string Registry { get; set; }

        public NetworkCredential RegistryCredentials { get; set; }

        public string Image { get; set; }

        public string Tag { get; set; }

        public string TagQualifiedImage => this.Image + ":" + this.Tag;

        public string RegistryQualifiedImage => this.Registry != null
            ? this.Registry + "/" + this.Image
            : this.Image;

        public string FullyQualifiedImage => this.Registry != null
            ? this.Registry + "/" + this.TagQualifiedImage
            : this.TagQualifiedImage;

        // Container related
        public string ContainerName { get; set; }

        public bool AutoRemoveContainerOnInitialization { get; set; }

        public bool AutoRemoveContainerOnDispose { get; set; }

        public DockerContainerPortBinding[] ContainerPortBindings { get; set; } = Array.Empty<DockerContainerPortBinding>();

        public string[] ContainerEnvironmentVariables { get; set; } = Array.Empty<string>();

        // Availability related
        public Func<CancellationToken, Task<bool>> WaitUntilAvailable { get; set; }

        public int MaximumWaitUntilAvailableAttempts { get; set; }

        public TimeSpan TimeBetweenWaitUntilAvailableAttempts { get; set; }
    }
}
