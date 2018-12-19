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
                .AddIdentityServerAuthentication(
                    IdentityServerAuthenticationDefaults.AuthenticationScheme,
                    options =>
                {
                    options.Authority = this.appSettings.IdentityServer.Authority;
                    options.Audience = this.appSettings.IdentityServer.Authority + "/resources";
                    options.RequireHttpsMetadata = false;
                },
                 moreoptions =>
                {
                    moreoptions.Authority = this.appSettings.IdentityServer.Authority;
                    moreoptions.DiscoveryPolicy = new DiscoveryPolicy { ValidateIssuerName = false };
                    moreoptions.ClientId = this.appSettings.IdentityServer.ApiName;
                    moreoptions.ClientSecret = this.appSettings.IdentityServer.ApiSecret;

                    //TODO Reference tokens don't seem to like caching turned on
                    moreoptions.EnableCaching = false;
                    moreoptions.CacheDuration = this.appSettings.IdentityServer.CacheTimeout;
                });

            services.AddCarter();

            services.AddProblemDetails(opts =>
            {
                opts.Map<VerificationCodeMismatch>(ex => new ProblemDetails
                {
                    Title = "The confirmation code is invalid",
                    Status = 400,
                    Type = "/verification-code-mismatch"
                });

                opts.Map<MaximumVerificationAttemptsReached>(ex => new ProblemDetails
                {
                    Title = "The confirmation code has been submitted too many times",
                    Status = 400,
                    Type = "/maximum-verification-attempts-reached"
                });

                opts.Map<VerificationRequestMissing>(ex => new ProblemDetails
                {
                    Title = "There is no verification request for the user",
                    Status = 400,
                    Type = "/verification-request-missing"
                });

                opts.Map<VerificationAlreadyStarted>(ex => new ProblemDetails
                {
                    Title = "There is already outstanding verification request",
                    Status = 400,
                    Type = "/verification-already-started"
                });

                opts.Map<VerificationCodeHasExpired>(ex => new ProblemDetails
                {
                    Title = "The verification code has expired",
                    Status = 400,
                    Type = "/verification-code-has-expired"
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ProblemDetailsMiddleware>();
            app.UseAuthentication();
            app.UseCarter();
        }
    }
}
