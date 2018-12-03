// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using Xbehave;

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

        [Scenario]
        public void Verifying_an_email_address(string code)
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { await this.driver.SendRequestToVerifyEmailAddress(); });

            "And I wait for the email confirmation code".x(async () => { code = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I submit the confirmation code to be verified".x(async () => { await this.driver.SubmitVerificationCode(code); });

            "Then my email verification status is true in Ironclad".x(() => { await this.driver.WaitForEmailToBeVerified();});
        }
        
        [Scenario]
        public Task Providing_incorrect_code_three_times()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "I can login as the user".x(async () => { await this.driver.Login(); });//
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
        
        [Scenario]
        public Task Providing_correct_code_after_three_incorrect_attempts()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "I can login as the user".x(async () => { await this.driver.Login(); });//
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
        
        [Scenario]
        public Task Providing_incorrect_code_four_times()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "I can login as the user".x(async () => { await this.driver.Login(); });//
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

        [Scenario]
        public Task Veryifying_email_without_verified_phone_number()
        {
//            await this.driver.RegisterUserWithUnverifiedPhoneNumber();
//
//            await this.driver.LoginAsUser();
//
//            await this.driver.SendRequestToVerifyEmailAddress();

            return Task.CompletedTask;
        }

        [Scenario]
        public Task Verifying_email_without_requesting_a_code()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "I can login as the user".x(async () => { await this.driver.Login(); });
//            var invalidCode = VerificationCode.Generate().ToString();
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);

            return Task.CompletedTask;
        }

        [Scenario]
        public Task Providing_incorrect_code_three_times_and_then_succeeding_with_new_code()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "I can login as the user".x(async () => { await this.driver.Login(); });
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
        
        [Scenario]
        public Task Providing_incorrect_code_three_times_and_request_new_code_and_try_with_old_code()
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "I can login as the user".x(async () => { await this.driver.Login(); });
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

        [Scenario]
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