using System;
using System.IO;
using Apollo.Features.Verification;
using Apollo.Features.Verification.Email;
using Apollo.Settings;
using Carter;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Apollo
{
    using Apollo.Persistence;
    using Apollo.Persistence.AzureStorage;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly AppSettings _appSettings = new AppSettings();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.Bind(_appSettings);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IVerificationRequestRepository>(x =>
                new VerificationRequestRepository(_appSettings.Db.DataConnString));

            services.AddSingleton(_appSettings);
            services.AddSingleton<MailSender>();
            services.AddSingleton<VerificationCodeManager>();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = _appSettings.IdentityServer.Authority;
                    options.ApiName = _appSettings.IdentityServer.ApiName;
                    options.ApiSecret = _appSettings.IdentityServer.ApiSecret;
                    options.RequireHttpsMetadata = false;

                    if (_appSettings.IdentityServer.CacheTimeout.TotalMilliseconds > 0)
                    {
                        options.EnableCaching = true;
                        options.CacheDuration = _appSettings.IdentityServer.CacheTimeout;
                    }
                });

            services.AddCarter();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseCarter();
        }
    }
}
