using Apollo.Persistence;
using Apollo.Persistence.AzureStorage;
using Apollo.Settings;
using Apollo.Swagger;
using Carter;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Apollo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly AppSettings _appSettings = new AppSettings();

        private const string ApiVersion = "v1";
        private const string ApiTitle = "Lykke KYC API";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.Bind(_appSettings);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCarter();

            services.AddSingleton<IVerificationRequestRepository>(x =>
                new VerificationRequestRepository(_appSettings.Db.DataConnString));

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = _appSettings.IdentityServer.Authority;
                    options.ApiName = _appSettings.IdentityServer.ApiName;
                    options.ApiSecret = _appSettings.IdentityServer.ApiSecret;

                    if (_appSettings.IdentityServer.CacheTimeout.TotalMilliseconds > 0)
                    {
                        options.EnableCaching = true;
                        options.CacheDuration = _appSettings.IdentityServer.CacheTimeout;
                    }
                });

#if DEBUG
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new Info {Title = ApiTitle, Version = ApiVersion});
                opt.DocumentFilter<SecurityRequirementsDocumentFilter>();
                opt.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    Scopes = _appSettings.Swagger.Security.OAuth2Scopes,
                    AuthorizationUrl = _appSettings.Swagger.Security.AuthorizeEndpoint
                });
            });
#endif
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCarter();

            app.UseAuthentication();

#if DEBUG
            app.UseSwaggerUI(opt =>
            {
                opt.RoutePrefix = "swagger/ui";
                opt.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiTitle);
                opt.OAuthClientId(_appSettings.Swagger.Security.OAuth2ClientId);
            });
#endif
        }
    }
}
