namespace Apollo.Features.Status
{
    using System.Threading.Tasks;
    using Carter;

    public class StatusModule : CarterModule
    {
        public StatusModule()
        {
            this.Get("/status", context => Task.CompletedTask);
        }
    }
}