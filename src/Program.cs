// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Events;

    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder<Startup>(args)
                .UseUrls("http://+:5006")
                .ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.Custom.json", optional: true))
                .UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location))
                .UseSerilog((context, configuration) =>
                    configuration.WriteTo.Console(outputTemplate: "[{InstanceId}] [{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                        .WriteTo.ApplicationInsightsTraces(context.Configuration.GetValue<string>("AppInsightsKey"))
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()
                        .ReadFrom.Configuration(context.Configuration))
                .Build();

            try
            {
                Log.Information("Starting webhost");
                await webHost.RunAsync();
                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
