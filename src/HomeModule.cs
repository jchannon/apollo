using Carter;
using Microsoft.AspNetCore.Http;

namespace Apollo
{
    public class HomeModule : CarterModule
    {
        public HomeModule()
        {
            this.Get("/", async context => await context.Response.WriteAsync("Hello World") );
        }
    }
}