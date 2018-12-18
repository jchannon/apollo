// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System;
    using Apollo.Features.Verification;
    using Apollo.Features.Verification.Email;
    using Apollo.Features.Verification.Phone;
    using Apollo.Persistence;
    using Apollo.Persistence.AzureStorage;
    using Apollo.Settings;
    using Carter;
    using IdentityModel.Client;
    using IdentityServer4.AccessTokenValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Twilio;

    public class Startup
    {
        private readonly AppSettings appSettings = new AppSettings();

        public Startup(IConfiguration configuration)
        {
            configuration.Bind(this.appSettings);

            TwilioClient.Init(this.appSettings.Twilio.AccountSid, this.appSettings.Twilio.AuthToken);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IVerificationRequestRepository>(x =>
                new VerificationRequestRepository(this.appSettings.Db.DataConnString));

            services.AddSingleton(this.appSettings);

            services.AddSingleton<IroncladClaimUpdater>();
            services.AddSingleton<MailSender>();
            services.AddSingleton<TwilioSender>();
            services.AddSingleton<VerificationCodeManager>();

            services.AddHttpClient(Constants.IroncladClient, client => { client.BaseAddress = new Uri(this.appSettings.IdentityServer.Authority); });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = this.appSettings.IdentityServer.Authority;
                    options.Audience = this.appSettings.IdentityServer.Authority + "/resources";
                    options.RequireHttpsMetadata = false;
                }, moreoptions =>
                {
                    moreoptions.Authority = this.appSettings.IdentityServer.Authority;
                    moreoptions.DiscoveryPolicy = new DiscoveryPolicy
                        { ValidateIssuerName = false };
                    moreoptions.ClientId = this.appSettings.IdentityServer.ApiName;
                    moreoptions.ClientSecret = this.appSettings.IdentityServer.ApiSecret;

                    //TODO Reference tokens don't seem to like caching turned on
                    moreoptions.EnableCaching = false;
                    moreoptions.CacheDuration = this.appSettings.IdentityServer.CacheTimeout;
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
