// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System.Net;
    using System.Net.Http;
    using Apollo.Features.Verification;
    using Apollo.Tests.Unit.Sdk;
    using FluentAssertions;
    using Xbehave;
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
        public void Verifying_an_email_address(string code, HttpResponseMessage verificationRequestResponse, HttpResponseMessage verificationSubmissionResponse)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { verificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I wait for the email confirmation code".x(async () => { code = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I submit the confirmation code to be verified".x(async () => { verificationSubmissionResponse = await this.driver.SubmitVerificationCode(code); });

            "And the code is accepted".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent));

            "Then my email verification status is true in Ironclad".x(async () => { await this.driver.WaitForEmailToBeVerified(); });
        }

        [Scenario]
        public void Providing_incorrect_code_three_times(string invalidCode, HttpResponseMessage verificationRequestResponse, HttpResponseMessage verificationSubmissionResponse)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { verificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "And I submit the invalid confirmation code to be verified".x(async () =>
            {
                verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode);
            });

            "Then the confirmation response is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest)); // todo problem+json parsing KYC-36

            "And I submit the invalid confirmation code to be verified a second time".x(async () =>
            {
                verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode);
            });

            "Then the confirmation response is rejected".x(() =>
                verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest)); // todo problem+json parsing

            "And I submit the invalid confirmation code to be verified a third time".x(async () =>
            {
                verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode);
            });

            "Then the confirmation response is rejected with a different error".x(() =>
                verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest)); // todo problem+json parsing
        }

        [Scenario]
        public void Providing_correct_code_after_three_incorrect_attempts(string validCode, HttpResponseMessage verificationRequestResponse, string invalidCode, HttpResponseMessage verificationSubmissionResponse)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { verificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I wait for the email confirmation code".x(async () => { validCode = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "And I send the invalid code".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "And When I submit the invalid code again".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "And When I submit the invalid code a third time".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "And when I submit the valid code".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(validCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));
        }

        [Scenario]
        public void Verifying_email_without_requesting_a_code(string invalidCode, HttpResponseMessage verificationSubmissionResponse)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "When I submit the confirmation code to be verified".x(async () => { verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode); });

            "Then the code is rejected".x(() =>
            {
                verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                // todo assert body contains correct problem+json info
            });
        }

        [Scenario]
        public void Providing_incorrect_code_three_times_and_then_succeeding_with_new_code(
            HttpResponseMessage verificationRequestResponse,
            HttpResponseMessage verificationSubmissionResponse,
            string invalidCode,
            string newCode)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { verificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I wait for the email confirmation code".x(async () => { await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "And I send the invalid code".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "And When I submit the invalid code again".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "And When I submit the invalid code a third time".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "When I request a new verification code for my email".x(async () => { verificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "The request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I wait for the email confirmation code".x(async () => { newCode = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I submit the confirmation code to be verified".x(async () => { verificationSubmissionResponse = await this.driver.SubmitVerificationCode(newCode); });

            "And the code is accepted".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent));

            "Then my email verification status is true in Ironclad".x(async () => { await this.driver.WaitForEmailToBeVerified(); });
        }

        [Scenario]
        public void Providing_incorrect_code_three_times_and_request_new_code_and_try_with_old_code(
            HttpResponseMessage verificationRequestResponse,
            string oldCode,
            HttpResponseMessage verificationSubmissionResponse,
            string invalidCode)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { verificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I wait for the email confirmation code".x(async () => { oldCode = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "And I send the invalid code".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "And When I submit the invalid code again".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "And When I submit the invalid code a third time".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "When I request a new verification code for my email".x(async () => { verificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "The request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I wait for the email confirmation code".x(async () => { await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I submit the old code to be verified".x(async () => { verificationSubmissionResponse = await this.driver.SubmitVerificationCode(oldCode); });

            "Then the code is rejected".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));
        }

        [Scenario]
        public void Requesting_a_new_code_when_old_code_is_still_valid(HttpResponseMessage verificationRequestResponse, string code, HttpResponseMessage invalidVerificationRequestResponse, HttpResponseMessage verificationSubmissionResponse)
        {
            "Given I have a user with an unverified email and unverified phone number".x(async () => { await this.driver.RegisterUser(); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { verificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I wait for the email confirmation code".x(async () => { code = await this.driver.WaitForEmailWithConfirmationCode(); });

            "And I try to request a new verificaiton code".x(async () => { invalidVerificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "Then the request is rejected".x(() => invalidVerificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "And I can submit the original code".x(async () => verificationSubmissionResponse = await this.driver.SubmitVerificationCode(code));

            "And the code is accepted".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent));

            "Then my email verification status is true in Ironclad".x(async () => { await this.driver.WaitForEmailToBeVerified(); });
        }

        [Scenario]
        public void Verifying_an_already_verified_email(HttpResponseMessage verificationRequestResponse)
        {
            "Given I have a user with a verified email".x(async () => { await this.driver.RegisterUser(true); });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I request to verify my email address".x(async () => { verificationRequestResponse = await this.driver.SendRequestToVerifyEmailAddress(); });

            "Then the request is rejected".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest));
        }
    }
}
