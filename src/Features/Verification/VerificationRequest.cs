namespace Apollo.Features.Verification
{
    using System;

    public class VerificationRequest
    {
        public VerificationRequest(string userId, VerificationType verificationType, VerificationRequestStatus status,
            DateTime expiryDate, VerificationCode code,
            int attempts)
        {
            this.UserId = userId;
            this.ExpiryDate = expiryDate;
            this.VerificationType = verificationType;
            this.Code = code;
            this.Status = status;
            this.Attempts = attempts;
        }

        public string UserId { get; set; }

        public DateTime ExpiryDate { get; set; }

        public VerificationType VerificationType { get; set; }

        public VerificationCode Code { get; set; }

        public VerificationRequestStatus Status { get; set; }

        public int Attempts { get; set; }

        public static VerificationRequest GenerateNewVerificationRequest(string userId, VerificationType vericationType, VerificationCode code)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            //todo configure expiry time
            return new VerificationRequest(userId, vericationType, VerificationRequestStatus.Pending, DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)), code, 0);
        }

        public bool IsActive()
        {
            switch (this.Status)
            {
                case VerificationRequestStatus.Confirmed:
                case VerificationRequestStatus.Pending when this.ExpiryDate > DateTime.UtcNow:
                    return true;
                default:
                    return false;
            }
        }

        public VerificationRequest ValidateCode(VerificationCode code)
        {
            //todo the logic is a bit dodgy here, should be properly immutable
            var validatedRequest = new VerificationRequest(this.UserId, this.VerificationType, this.Status, this.ExpiryDate, this.Code, this.Attempts);
            validatedRequest.Attempts++;

            if (this.Status != VerificationRequestStatus.Pending)
            {
                return validatedRequest;
            }
            
            if (this.ExpiryDate < DateTime.UtcNow)
            {
                validatedRequest.Status = VerificationRequestStatus.Expired;
                return validatedRequest;
            }

            if (this.Code.Equals(code))
            {
                validatedRequest.Status = VerificationRequestStatus.Confirmed;
            }
            else if (validatedRequest.Attempts == 3)
            {
                validatedRequest.Status = VerificationRequestStatus.Failed;
            }

            return validatedRequest;
        }
    }
}
