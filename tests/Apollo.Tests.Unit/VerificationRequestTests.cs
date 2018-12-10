namespace Apollo.Tests.Unit
{
    using System;
    using Apollo.Features.Verification;
    using FluentAssertions;
    using Xunit;

    public class VerificationRequestTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UserIdCannotBeNullOrEmpty(string userId)
        {
            Assert.Throws<ArgumentNullException>(() => VerificationRequest.GenerateNewVerificationRequest(userId, VerificationType.Email, null));
        }

        [Fact]
        public void CanCreateValidVerificationRequest()
        {
            var request = VerificationRequest.GenerateNewVerificationRequest("userId", VerificationType.Email, VerificationCode.Generate());

            request.Should().NotBeNull();
            request.Attempts.Should().Be(0);
            request.Status.Should().Be(VerificationRequestStatus.Pending);
            request.ExpiryDate.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public void VerificationCodeCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => VerificationRequest.GenerateNewVerificationRequest("userId", VerificationType.PhoneCall, null));
        }

        [Fact]
        public void ShouldRejectExpiredCodes()
        {
            var code = VerificationCode.Generate();
            var expiredRequest = new VerificationRequest("userId", VerificationType.SMS, VerificationRequestStatus.Pending, DateTime.MinValue, code, 0);

            var validatedRequest = expiredRequest.ValidateCode(code);

            validatedRequest.Status.Should().Be(VerificationRequestStatus.Expired);
        }

        [Fact]
        public void ShouldRejectFailedCode()
        {
            var code = VerificationCode.Generate();
            var failedRequest = new VerificationRequest("userId", VerificationType.SMS, VerificationRequestStatus.Failed, DateTime.MinValue, code, 3);

            var validatedRequest = failedRequest.ValidateCode(code);

            validatedRequest.Status.Should().Be(VerificationRequestStatus.Failed);
        }

        [Fact]
        public void ShouldFailCodeAfterThreeAttempts()
        {
            var code = VerificationCode.Generate();
            var failedRequest = new VerificationRequest("userId", VerificationType.SMS, VerificationRequestStatus.Pending, DateTime.MaxValue, code, 2);

            var validatedRequest = failedRequest.ValidateCode(VerificationCode.Generate());

            validatedRequest.Status.Should().Be(VerificationRequestStatus.Failed);
        }
        
        [Fact]
        public void ShouldAcceptCodeOnLastAttempt()
        {
            var code = VerificationCode.Generate();
            var failedRequest = new VerificationRequest("userId", VerificationType.SMS, VerificationRequestStatus.Pending, DateTime.MaxValue, code, 2);

            var validatedRequest = failedRequest.ValidateCode(code);

            validatedRequest.Status.Should().Be(VerificationRequestStatus.Confirmed);
        }
    }
}
