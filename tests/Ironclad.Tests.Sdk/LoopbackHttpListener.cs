﻿// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable CA1056

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    internal sealed class LoopbackHttpListener : IDisposable
    {
        private const int DefaultTimeout = 30;

        private readonly IWebHost host;
        private readonly TaskCompletionSource<string> source = new TaskCompletionSource<string>();

        public LoopbackHttpListener(int port, string path = null)
        {
            path = path ?? string.Empty;

            if (path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(1);
            }

            this.Url = $"http://127.0.0.1:{port}/{path}";
            this.host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(this.Url)
                .Configure(this.Configure)
                .Build();

            this.host.Start();
        }

        public string Url { get; }

        public Task<string> WaitForCallbackAsync(int timeoutInSeconds = DefaultTimeout)
        {
            Task.Run(
                async () =>
                {
                    await Task.Delay(timeoutInSeconds * 1000).ConfigureAwait(false);
                    this.source.TrySetCanceled();
                });

            return this.source.Task;
        }

        public void Dispose()
        {
            Task.Run(
                async () =>
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    this.host.Dispose();
                });
        }

        private void Configure(IApplicationBuilder app)
        {
            app.Run(
                async ctx =>
                {
                    if (ctx.Request.Method == "GET")
                    {
                        this.SetResult(ctx.Request.QueryString.Value, ctx);
                    }
                    else if (ctx.Request.Method == "POST")
                    {
                        if (!ctx.Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                        {
                            ctx.Response.StatusCode = 415;
                        }
                        else
                        {
                            using (var sr = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                            {
                                var body = await sr.ReadToEndAsync().ConfigureAwait(false);
                                this.SetResult(body, ctx);
                            }
                        }
                    }
                    else
                    {
                        ctx.Response.StatusCode = 405;
                    }
                });
        }

        private void SetResult(string value, HttpContext ctx)
        {
            try
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
                ctx.Response.Body.Flush();

                this.source.TrySetResult(value);
            }
            catch
            {
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
                ctx.Response.Body.Flush();
            }
        }
    }
}
