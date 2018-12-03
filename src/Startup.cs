namespace Apollo
{
    using System;
    using Carter;
    using IdentityServer4.AccessTokenValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public readonly AppSettings appSettings = new AppSettings();

        public Startup(IConfiguration configuration)
        {
            configuration.Bind(this.appSettings);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(this.appSettings);
            
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = this.appSettings.IdentityServer.Authority;
                    options.ApiName = this.appSettings.IdentityServer.ApiName;
                    options.ApiSecret = this.appSettings.IdentityServer.ApiSecret;
                    
                    options.RequireHttpsMetadata = false;
                    if (this.appSettings.IdentityServer.CacheTimeout.TotalMilliseconds > 0)
                    {
                        options.EnableCaching = true;
                        options.CacheDuration = this.appSettings.IdentityServer.CacheTimeout;
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

    public class AppSettings
    {
        public IdentityServerSettings IdentityServer { get; set; }
    }

    public class IdentityServerSettings
    {
        public string Authority { get; set; }

        public string ApiName { get; set; }

        public string ApiSecret { get; set; }

        public TimeSpan CacheTimeout { get; set; }
    }
}
