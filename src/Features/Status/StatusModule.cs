// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Status
{
    using System.Threading.Tasks;
    using Carter;

    public class StatusModule : CarterModule
    {
        public StatusModule()
        {
            this.Get("/status", context => Task.CompletedTask); // todo KYC-45
        }
    }
}
