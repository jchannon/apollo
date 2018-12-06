namespace Apollo
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using System.IO;

    class Program
    {
        static async Task Main(string[] args)
        {
            await WebHost.CreateDefaultBuilder<Startup>(args)
                .UseUrls("http://+:5006")
                .UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location))
                .Build()
                .RunAsync();
        }
    }
}
