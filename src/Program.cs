namespace Apollo
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await WebHost.CreateDefaultBuilder<Startup>(args)
                .UseUrls("http://+:5006")
                .UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location))
                .Build()
                .RunAsync();
        }
    }
}
