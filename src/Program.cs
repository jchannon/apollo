using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Apollo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await WebHost.CreateDefaultBuilder<Startup>(args)
                .UseUrls("http://+:5006")
                .Build()
                
                .RunAsync();
        }
    }
}