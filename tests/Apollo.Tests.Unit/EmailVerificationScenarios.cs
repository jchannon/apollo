// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System;
    using System.Threading.Tasks;
    using Sdk;
    using Ironclad.Tests.Sdk;
    using Xunit;

    [Collection(nameof(ApolloIntegrationCollection))]
    public class EmailVerificationScenarios
    {
        private readonly Driver driver;

        public EmailVerificationScenarios(ApolloIntegrationFixture services, AuthenticationFixture authentication)
        {
            this.driver = new Driver(services, authentication);
        }

        [Fact]
        public Task Verifying_an_email_address()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyEmailAddress();
//
//            var code = await this.driver.WaitForEmailWithCodeToArrive();
//
//            await this.driver.SendEmailConfirmationCode(code);
//
//            await this.driver.WaitForEmailVerifiedToBeSetInIronclad();
            return Task.CompletedTask;
        }
        
        private class Driver
        {
            private readonly ApolloIntegrationFixture services;
            private readonly AuthenticationFixture authentication;

            public Driver(ApolloIntegrationFixture services, AuthenticationFixture authentication)
            {
                this.services = services ?? throw new ArgumentNullException(nameof(services));
                this.authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            }
        }
    }   
}