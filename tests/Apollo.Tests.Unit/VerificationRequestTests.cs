namespace Apollo.Tests.Unit
{
    using System;
    using Persistence;
    using Xunit;

    public class VerificationRequestTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateEmailVerificationRequest_InvalidUserId_RaisesException(string lykkeUserId)
        {
            Assert.Throws<ArgumentNullException>(() =>
                VerificationRequestDto.CreateEmailVerificationRequest(lykkeUserId, DateTime.UtcNow.AddDays(1)));
        }

        [Fact]
        public void CreateEmailVerificationRequest_InvalidExpirationDate_RaisesException()
        {
            var lykkeUserId = Guid.NewGuid().ToString();

            var invalidExpirationDate = DateTime.UtcNow.AddDays(-1);

            Assert.Throws<InvalidOperationException>(() =>
                VerificationRequestDto.CreateEmailVerificationRequest(lykkeUserId, invalidExpirationDate));
        }

        [Fact]
        public void CreateEmailVerificationRequest_ValidParameters_CreatesRequest()
        {
            var lykkeUserId = Guid.NewGuid().ToString();

            var expirationDate = DateTime.UtcNow.AddDays(1);

            var verificationRequest = VerificationRequestDto.CreateEmailVerificationRequest(lykkeUserId, expirationDate);

            Assert.Equal(VerificationRequestStatus.Accepted, verificationRequest.Status);
            Assert.Equal(lykkeUserId, verificationRequest.LykkeUserId);
            Assert.NotNull(verificationRequest.Code);
            Assert.Equal(VerificationRequestMethod.SendEmail, verificationRequest.Method);
            Assert.Equal(VerificationRequestSubject.Email, verificationRequest.Subject);
            Assert.Equal(expirationDate, verificationRequest.ExpirationDate);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreatePhoneVerificationRequest_InvalidUserId_RaisesException(string lykkeUserId)
        {
            Assert.Throws<ArgumentNullException>(() => VerificationRequestDto.CreatePhoneVerificationRequest(
                lykkeUserId,
                DateTime.UtcNow.AddDays(1),
                VerificationRequestMethod.SendSms));
        }

        [Fact]
        public void CreatePhoneVerificationRequest_InvalidExpirationDate_RaisesException()
        {
            var lykkeUserId = Guid.NewGuid().ToString();

            var invalidExpirationDate = DateTime.UtcNow.AddDays(-1);

            Assert.Throws<InvalidOperationException>(() => VerificationRequestDto.CreatePhoneVerificationRequest(
                lykkeUserId,
                invalidExpirationDate,
                VerificationRequestMethod.SendSms));
        }

        [Fact]
        public void CreatePhoneVerificationRequest_InvalidMethod_RaisesException()
        {
            var lykkeUserId = Guid.NewGuid().ToString();

            var expirationDate = DateTime.UtcNow.AddDays(1);

            Assert.Throws<InvalidOperationException>(() => VerificationRequestDto.CreatePhoneVerificationRequest(
                lykkeUserId,
                expirationDate,
                VerificationRequestMethod.SendEmail));
        }

        [Theory]
        [InlineData(VerificationRequestMethod.SendSms)]
        [InlineData(VerificationRequestMethod.PhoneCall)]
        public void CreatePhoneVerificationRequest_ValidParameters_CreatesRequest(VerificationRequestMethod method)
        {
            var lykkeUserId = Guid.NewGuid().ToString();

            var expirationDate = DateTime.UtcNow.AddDays(1);

            var verificationRequest =
                VerificationRequestDto.CreatePhoneVerificationRequest(lykkeUserId, expirationDate, method);

            Assert.Equal(VerificationRequestStatus.Accepted, verificationRequest.Status);
            Assert.Equal(lykkeUserId, verificationRequest.LykkeUserId);
            Assert.NotNull(verificationRequest.Code);
            Assert.Equal(method, verificationRequest.Method);
            Assert.Equal(VerificationRequestSubject.Phone, verificationRequest.Subject);
            Assert.Equal(expirationDate, verificationRequest.ExpirationDate);
        }
    }
}
