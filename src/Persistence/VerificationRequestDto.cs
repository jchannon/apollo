using System;

namespace Apollo.Persistence
{
    using Apollo.Features.Verification;

    public class VerificationRequestDto
    {
        public string Id { get; set; }

        public string LykkeUserId { get; set; }

        public VerificationRequestStatus Status { get; set; }

        public DateTime ExpirationDate { get; set; }

        public VerificationRequestSubject Subject { get; set; }

        public VerificationRequestMethod Method { get; set; }

        public string Code { get; set; }

        public static VerificationRequestDto CreateEmailVerificationRequest(string lykkeUserId,
            DateTime expirationDate)
        {
            if (string.IsNullOrWhiteSpace(lykkeUserId))
                throw new ArgumentNullException(nameof(lykkeUserId));

            if (expirationDate < DateTime.UtcNow)
                throw new InvalidOperationException("Expiration date invalid value");

            return new VerificationRequestDto
            {
                Status = VerificationRequestStatus.Accepted,
                Method = VerificationRequestMethod.SendEmail,
                Code = VerificationCode.Generate().ToString(),
                LykkeUserId = lykkeUserId,
                ExpirationDate = expirationDate,
                Subject = VerificationRequestSubject.Email
            };
        }

        public static VerificationRequestDto CreatePhoneVerificationRequest(string lykkeUserId,
            DateTime expirationDate, VerificationRequestMethod method)
        {
            if (string.IsNullOrWhiteSpace(lykkeUserId))
                throw new ArgumentNullException(nameof(lykkeUserId));

            if (expirationDate < DateTime.UtcNow)
                throw new InvalidOperationException("Expiration date invalid value");

            if (method == VerificationRequestMethod.SendEmail)
                throw new InvalidOperationException(
                    $"Method {nameof(VerificationRequestMethod.SendEmail)} can't be used for phone verification");

            return new VerificationRequestDto
            {
                Status = VerificationRequestStatus.Accepted,
                Method = method,
                Code = VerificationCode.Generate().ToString(),
                LykkeUserId = lykkeUserId,
                ExpirationDate = expirationDate,
                Subject = VerificationRequestSubject.Phone
            };
        }
    }
}
