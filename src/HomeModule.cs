using System.Threading.Tasks;
using Carter;
using Microsoft.AspNetCore.Http;

namespace Apollo
{
    public class HomeModule : CarterModule
    {
        public HomeModule()
        {
            this.Get("/", async context => await context.Response.WriteAsync("Hello World") );
            this.Post("/email-verification-request", context =>
            {
                context.Response.StatusCode = 204;
                return Task.CompletedTask;
            });
        }
    }
}