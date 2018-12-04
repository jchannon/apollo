using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Apollo.Tests.Unit.Sdk
{
    public class BuiltFromSourceApollo : IAsyncLifetime
    {
        private readonly Uri apolloUri;
        private Process process;

        public BuiltFromSourceApollo(Uri apolloUri)
        {
            this.apolloUri = apolloUri;
        }

        public async Task InitializeAsync()
        {
            var path = string.Format(
                CultureInfo.InvariantCulture,
                "..{0}..{0}..{0}..{0}..{0}src{0}Apollo.csproj",
                Path.DirectorySeparatorChar);

            var arguments = $"run -p {path}";

            this.process = Process.Start(
                new ProcessStartInfo(
                    "dotnet",
                    arguments)
                {
                    UseShellExecute = !Environment.OSVersion.Platform.Equals(PlatformID.Unix)
                });

            async Task<bool> WaitUntilAvailable(CancellationToken token)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        using (var response = await client.GetAsync(this.apolloUri, token)
                            .ConfigureAwait(false))
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
            }

            const int maximumWaitUntilAvailableAttempts = 15;
            var timeBetweenWaitUntilAvailableAttempts = TimeSpan.FromSeconds(1);
            var attempt = 0;
            while (
                attempt < maximumWaitUntilAvailableAttempts &&
                !await WaitUntilAvailable(default).ConfigureAwait(false))
            {
                if (attempt != maximumWaitUntilAvailableAttempts - 1)
                {
                    await Task
                        .Delay(timeBetweenWaitUntilAvailableAttempts, default)
                        .ConfigureAwait(false);
                }

                attempt++;
            }

            if (attempt == maximumWaitUntilAvailableAttempts)
            {
                throw new Exception(
                    "The Apollo instance did not become available in a timely fashion.");
            }
        }

        public Task DisposeAsync()
        {
            if (this.process != null)
            {
                try
                {
                    if (Environment.OSVersion.Platform.Equals(PlatformID.Unix))
                    {
                        using (var killer = Process.Start(new ProcessStartInfo("pkill", $"-TERM -P {this.process.Id}")))
                        {
                            killer?.WaitForExit();
                        }
                    }
                    else
                    {
                        using (var killer =
                            Process.Start(new ProcessStartInfo("taskkill", $"/T /F /PID {this.process.Id}")))
                        {
                            killer?.WaitForExit();
                        }
                    }
                }
                catch (Win32Exception)
                {
                }
            }

            this.process?.Dispose();
            return Task.CompletedTask;
        }
    }
}