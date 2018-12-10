namespace Apollo
{
    using Apollo.Features.Verification;
    using Apollo.Features.Verification.Email;
    using Apollo.Features.Verification.Phone.PhoneVerification;
    using Apollo.Persistence;
    using Apollo.Persistence.AzureStorage;
    using Apollo.Settings;
    using Carter;
    using IdentityServer4.AccessTokenValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        private readonly AppSettings appSettings = new AppSettings();

        public Startup(IConfiguration configuration)
        {
            configuration.Bind(this.appSettings);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IVerificationRequestRepository>(x =>
                new VerificationRequestRepository(this.appSettings.Db.DataConnString));

            services.AddSingleton(this.appSettings);

            services.AddSingleton(s => new Handler(new ICommandHandler[]
            {
                new PhoneVerificationCommandHandler()
            }));

            services.AddSingleton<MailSender>();
            services.AddSingleton<VerificationCodeManager>();
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
}
