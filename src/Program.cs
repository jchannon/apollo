namespace Apollo
{
    using System.IO;
    using System.Threading.Tasks;
    using IdentityModel;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await WebHost.CreateDefaultBuilder<Startup>(args)
                .UseUrls("http://+:5006")
                .ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.Custom.json", optional: true))
                .UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location))
                .Build()
                .RunAsync();
        }
    }
}
