// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;

    public class ProblemDetailsMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ProblemDetailsOptions options;

        private readonly IHostingEnvironment environment;

        public ProblemDetailsMiddleware(
            RequestDelegate next,
            ProblemDetailsOptions options,
            IHostingEnvironment environment)
        {
            this.next = next;
            this.options = options;
            this.environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                if (this.options.TryMapProblemDetails(ex, out var result))
                {
                    if (result.Type.StartsWith("/"))
                    {
                        var uriBuilder = new UriBuilder(context.Request.Scheme, context.Request.Host.Value);
                        if (context.Request.Host.Port.HasValue)
                        {
                            uriBuilder.Port = context.Request.Host.Port.Value;
                        }

                        uriBuilder.Path = "/.problem" + result.Type;
                        result.Type = uriBuilder.ToString();
                    }

                    await context.Response.WriteProblemDetails(result);

                    return;
                }

                var exceptionDetails = new ExceptionProblemDetails(ex);

                if (this.environment.IsDevelopment())
                {
                    await context.Response.WriteProblemDetails(exceptionDetails);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
