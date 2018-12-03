// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System.Threading.Tasks;
    using Sdk;
    using Ironclad.Tests.Sdk;
    using Xunit;

    [Collection(nameof(ApolloIntegrationCollection))]
    public class EmailVerificationScenarios
    {
        private readonly MailDriver driver;

        public EmailVerificationScenarios(ApolloIntegrationFixture services)
        {
            this.driver = new MailDriver(services);
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
//            var code = await this.driver.WaitForEmailWithCode();
//
//            await this.driver.SendEmailConfirmationCode(code);
//
//            await this.driver.WaitForEmailVerifiedToBeSetInIronclad();
            return Task.CompletedTask;
        }
        
        [Fact]
        public Task Providing_incorrect_code_three_times()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyEmailAddress();
//
//            var code = await this.driver.WaitForEmailWithCode();
//
//            var invalidCode = VerificationCode.Generate().ToString();
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);

            return Task.CompletedTask;
        }
        
        [Fact]
        public Task Providing_correct_code_after_three_incorrect_attempts()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyEmailAddress();
//
//            var code = await this.driver.WaitForEmailWithCode();
//
//            var invalidCode = VerificationCode.Generate().ToString();
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(code);

            return Task.CompletedTask;
        }
        
        [Fact]
        public Task Providing_incorrect_code_four_times()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyEmailAddress();
//
//            var code = await this.driver.WaitForEmailWithCode();
//
//            var invalidCode = VerificationCode.Generate().ToString();
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);

            return Task.CompletedTask;
        }

        [Fact]
        public Task Veryifying_email_without_verified_phone_number()
        {
//            await this.driver.RegisterUserWithUnverifiedPhoneNumber();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyEmailAddress();

            return Task.CompletedTask;
        }

        [Fact]
        public Task Verifying_email_without_requesting_a_code()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            var invalidCode = VerificationCode.Generate().ToString();
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);

            return Task.CompletedTask;
        }

        [Fact]
        public Task Providing_incorrect_code_three_times_and_then_succeeding_with_new_code()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyEmailAddress();
//
//            var oldCode = await this.driver.WaitForEmailWithCode();
//
//            var invalidCode = VerificationCode.Generate().ToString();
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendRequestToVerifyEmailAddress();
//
//            var newCode = await this.driver.WaitForEmailWithCode();
//
//            await this.driver.SendEmailConfirmationCode(newCode);
//
//            await this.driver.WaitForEmailVerifiedToBeSetInIronclad();

            return Task.CompletedTask;
        }
        
        [Fact]
        public Task Providing_incorrect_code_three_times_and_request_new_code_and_try_with_old_code()
        {
//            await this.driver.RegisterUser();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyEmailAddress();
//
//            var oldCode = await this.driver.WaitForEmailWithCode();
//
//            var invalidCode = VerificationCode.Generate().ToString();
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendRequestToVerifyEmailAddress();
//
//            var newCode = await this.driver.WaitForEmailWithCode();
//
//            await this.driver.SendEmailConfirmationCode(oldCode);

            return Task.CompletedTask;
        }

        [Fact]
        public Task Verifying_an_already_verified_email()
        {
//            await this.driver.RegisterUserWithVerifiedEmail();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyEmailAddress();
            
            return Task.CompletedTask;
        }
    }
}