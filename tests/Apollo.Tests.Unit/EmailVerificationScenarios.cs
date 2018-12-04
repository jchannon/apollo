// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System;
    using System.Net;
    using System.Net.Http;
    using FluentAssertions;
    using Xbehave;    using Sdk;
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
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { await this.driver.SendRequestToVerifyEmailAddress(); });

            "And I wait for the email confirmation code".x(async () => { code = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I submit the confirmation code to be verified".x(async () => { await this.driver.SubmitVerificationCode(code); });

            "Then my email verification status is true in Ironclad".x(async () => { await this.driver.WaitForEmailToBeVerified(); });
        }

        [Scenario]
        public void Providing_incorrect_code_three_times(string invalidCode)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); }); //

            "When I request to verify my email address".x(async () => { await this.driver.SendRequestToVerifyEmailAddress(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "And I submit the invalid confirmation code to be verified".x(async () => { await this.driver.SubmitVerificationCode(invalidCode); });
            
            //todo check the response and send it some more times.
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
        }

        [Scenario]
        public void Providing_correct_code_after_three_incorrect_attempts(string code, string invalidCode)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { await this.driver.SendRequestToVerifyEmailAddress(); });

            "And I wait for the email confirmation code".x(async () => { code = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(code);
        }

        [Scenario]
        public void Providing_incorrect_code_four_times(string invalidCode)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { await this.driver.SendRequestToVerifyEmailAddress(); });

            "And I wait for the email confirmation code".x(async () => { await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
//
//            await this.driver.SendEmailConfirmationCode(invalidCode);
        }

        [Scenario]
        public void Verifying_email_without_requesting_a_code(string invalidCode, HttpResponseMessage responseMessage)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });
            
            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "When I submit the confirmation code to be verified".x(async () => { await this.driver.SubmitVerificationCode(invalidCode); });

            "Then the code is rejected".x(() =>
            {
                responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                //todo assert body contains correct problem+json info
            });

            "And the email is not verified".x(async () => { });
        }

        [Scenario]
        public void Providing_incorrect_code_three_times_and_then_succeeding_with_new_code(string oldCode, string invalidCode, string newCode)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { await this.driver.SendRequestToVerifyEmailAddress(); });
            
            "And I wait for the email confirmation code".x(async () => { oldCode = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

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
        }

        [Scenario]
        public void Providing_incorrect_code_three_times_and_request_new_code_and_try_with_old_code(string oldCode, string invalidCode)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { await this.driver.SendRequestToVerifyEmailAddress(); });
            
            "And I wait for the email confirmation code".x(async () => { oldCode = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });
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
        }

        [Scenario]
        public void Verifying_an_already_verified_email()
        {
            "Given I have a user with a verified email".x(async () => { await this.driver.RegisterUser(emailVerified: true); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { await this.driver.SendRequestToVerifyEmailAddress(); });

            "Then the request is rejected".x(() =>
            {
                throw new NotImplementedException();
            });
        }
    }
}