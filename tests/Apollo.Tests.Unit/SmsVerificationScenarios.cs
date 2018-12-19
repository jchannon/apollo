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
    public class SmsVerificationScenarios
    {
        private readonly SmsDriver smsDriver;

        public SmsVerificationScenarios(ApolloIntegrationFixture services)
        {
            this.smsDriver = new SmsDriver(services);
        }

        [Scenario]
        public void Verifying_a_phone_number(HttpResponseMessage smsResponse, string code, HttpResponseMessage submissionResponse)
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.smsDriver.RegisterUser(emailVerified: true, phoneVerified: false); });

            "And I can login as the user".x(async () => { await this.smsDriver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { smsResponse = await this.smsDriver.RequestSMSCode(); });

            "And the request is accepted".x(() => { smsResponse.StatusCode.Should().Be(HttpStatusCode.Accepted); });

            "Then I receive an SMS with the verification code in".x(async () => { code = await this.smsDriver.WaitForSMS(); });

            "And I can submit that code to verify my phone number".x(async () => { submissionResponse = await this.smsDriver.SubmitVerificationCode(code); });

            "And the code is accepted".x(() => { submissionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent); });

            "Then my phone number is verified".x(async () => { (await this.smsDriver.CheckPhoneIsVerified()).Should().BeTrue(); });
        }

        [Scenario]
        public void Providing_incorrect_code_three_times(string invalidCode, HttpResponseMessage verificationRequestResponse, HttpResponseMessage verificationSubmissionResponse)
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.smsDriver.RegisterUser(emailVerified: true, phoneVerified: false); });

            "And I can login as the user".x(async () => { await this.smsDriver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { verificationRequestResponse = await this.smsDriver.RequestSMSCode(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "And I submit the invalid confirmation code to be verified".x(async () => { verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode); });

            "Then the code is rejected for mismatched codes".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-code-mismatch"));

            "And I submit the invalid confirmation code to be verified a second time".x(async () => { verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode); });

            "Then the code is rejected for mismatched codes".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-code-mismatch"));

            "And I submit the invalid confirmation code to be verified a third time".x(async () => { verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode); });

            "Then the code is rejected for max attempts".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/maximum-verification-attempts-reached"));
        }

        [Scenario]
        public void Providing_correct_code_after_three_incorrect_attempts(string validCode, HttpResponseMessage verificationRequestResponse, string invalidCode, HttpResponseMessage verificationSubmissionResponse)
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.smsDriver.RegisterUser(emailVerified: true, phoneVerified: false); });

            "And I can login as the user".x(async () => { await this.smsDriver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { verificationRequestResponse = await this.smsDriver.RequestSMSCode(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "Then I receive an SMS with the verification code in".x(async () => { validCode = await this.smsDriver.WaitForSMS(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "And I send the invalid code".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected for mismatched codes".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-code-mismatch"));

            "And When I submit the invalid code again".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected for mismatched codes".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-code-mismatch"));

            "And When I submit the invalid code a third time".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected for max attempts".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/maximum-verification-attempts-reached"));

            "And when I submit the valid code".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(validCode));

            "Then the code is rejected for max attempts".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/maximum-verification-attempts-reached"));
        }

        [Scenario]
        public void Verifying_sms_without_requesting_a_code(string invalidCode, HttpResponseMessage verificationSubmissionResponse)
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.smsDriver.RegisterUser(emailVerified: true, phoneVerified: false); });

            "And I can login as the user".x(async () => { await this.smsDriver.Login(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "When I submit the confirmation code to be verified".x(async () => { verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode); });

            "Then the code is rejected because the request is missing".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-request-missing"));
        }

        [Scenario]
        public void Providing_incorrect_code_three_times_and_then_succeeding_with_new_code(
            HttpResponseMessage verificationRequestResponse,
            HttpResponseMessage verificationSubmissionResponse,
            string invalidCode,
            string newCode)
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.smsDriver.RegisterUser(emailVerified: true, phoneVerified: false); });

            "And I can login as the user".x(async () => { await this.smsDriver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { verificationRequestResponse = await this.smsDriver.RequestSMSCode(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "Then I receive an SMS with the verification code in".x(async () => { await this.smsDriver.WaitForSMS(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "And I send the invalid code".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected for mismatched codes".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-code-mismatch"));

            "And When I submit the invalid code again".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected for mismatched codes".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-code-mismatch"));

            "And When I submit the invalid code a third time".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected for max attempts".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/maximum-verification-attempts-reached"));

            "When I request a new code to be sent via SMS to verify my phone number".x(async () => { verificationRequestResponse = await this.smsDriver.RequestSMSCode(); });

            "The request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "Then I receive an SMS with the verification code in".x(async () => { newCode = await this.smsDriver.WaitForSMS(); });

            "And I submit the confirmation code to be verified".x(async () => { verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(newCode); });

            "And the code is accepted".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent));

            "Then my phone verification status is true in Ironclad".x(async () => { (await this.smsDriver.CheckPhoneIsVerified()).Should().BeTrue(); });
        }

        [Scenario]
        public void Providing_incorrect_code_three_times_and_request_new_code_and_try_with_old_code(
            HttpResponseMessage verificationRequestResponse,
            string oldCode,
            HttpResponseMessage verificationSubmissionResponse,
            string invalidCode)
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.smsDriver.RegisterUser(emailVerified: true, phoneVerified: false); });

            "And I can login as the user".x(async () => { await this.smsDriver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { verificationRequestResponse = await this.smsDriver.RequestSMSCode(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "Then I receive an SMS with the verification code in".x(async () => { oldCode = await this.smsDriver.WaitForSMS(); });

            "And I have an invalid code".x(() => { invalidCode = VerificationCode.Generate().ToString(); });

            "And I send the invalid code".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected for mismatched codes".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-code-mismatch"));

            "And When I submit the invalid code again".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected for mismatched codes".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-code-mismatch"));

            "And When I submit the invalid code a third time".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(invalidCode));

            "Then the code is rejected for max attempts".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/maximum-verification-attempts-reached"));

            "When I request a new verification code for my phone number".x(async () => { verificationRequestResponse = await this.smsDriver.RequestSMSCode(); });

            "The request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I wait for the confirmation code".x(async () => { await this.smsDriver.WaitForSMS(); });

            "And I submit the old code to be verified".x(async () => { verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(oldCode); });

            "Then the code is rejected for mismatched codes".x(async () => await verificationSubmissionResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-code-mismatch"));
        }

        [Scenario]
        public void Requesting_a_new_code_when_old_code_is_still_valid(
            HttpResponseMessage verificationRequestResponse,
            string code,
            HttpResponseMessage invalidVerificationRequestResponse,
            HttpResponseMessage verificationSubmissionResponse)
        {
            "Given I have a user with a verified email and unverified phone number".x(async () => { await this.smsDriver.RegisterUser(emailVerified: true, phoneVerified: false); });

            "And I can login as the user".x(async () => { await this.smsDriver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { verificationRequestResponse = await this.smsDriver.RequestSMSCode(); });

            "And the request is accepted".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted));

            "And I wait for the SMS confirmation code".x(async () => { code = await this.smsDriver.WaitForSMS(); });

            "And I try to request a new verification code".x(async () => { invalidVerificationRequestResponse = await this.smsDriver.RequestSMSCode(); });

            "Then the request is rejected for too many requests open".x(async () => await invalidVerificationRequestResponse.ShouldBeRejectedWithMatchingTypeField("http://localhost/.problem/verification-already-started"));

            "And I can submit the original code".x(async () => verificationSubmissionResponse = await this.smsDriver.SubmitVerificationCode(code));

            "And the code is accepted".x(() => verificationSubmissionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent));

            "Then my phone verification status is true in Ironclad".x(async () => { (await this.smsDriver.CheckPhoneIsVerified()).Should().BeTrue(); });
        }

        [Scenario]
        public void Verifying_an_already_verified_sms(HttpResponseMessage verificationRequestResponse)
        {
            "Given I have a user with a verified email and phone".x(async () => { await this.smsDriver.RegisterUser(emailVerified: true, phoneVerified: true); });

            "And I can login as the user".x(async () => { await this.smsDriver.Login(); });

            "When I request a code to be sent via SMS to verify my phone number".x(async () => { verificationRequestResponse = await this.smsDriver.RequestSMSCode(); });

            "Then the request is rejected".x(() => verificationRequestResponse.StatusCode.Should().Be(HttpStatusCode.NoContent));
        }
    }
}
