// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification
{
    using System;
    using System.Threading.Tasks;
    using Apollo.Persistence;

    public class VerificationCodeManager
    {
        private readonly IVerificationRequestRepository verificationRequestRepository;

        public VerificationCodeManager(IVerificationRequestRepository verificationRequestRepository)
        {
            this.verificationRequestRepository = verificationRequestRepository;
        }

        public async Task<bool> GenerateCode(VerificationType type, string userId, Func<VerificationCode, Task> onSuccessfulGeneration)
        {
            var outstandingRequest = await this.verificationRequestRepository.GetVerificationRequest(type, userId);
            if (outstandingRequest != null && outstandingRequest.IsActive())
            {
                return false;
            }

            var verificationRequest = VerificationRequest.GenerateNewVerificationRequest(userId, type, VerificationCode.Generate());

            await this.verificationRequestRepository.StoreNewVerificationRequest(verificationRequest); // todo config the time KYC-43

            try
            {
                await onSuccessfulGeneration(verificationRequest.Code);
            }
            catch (SenderException)
            {
                // todo add logging KYC-41
                return false;
            }

            return true;
        }

        public async Task<bool> VerifyCode(VerificationType type, string userId, VerificationCode code)
        {
            var storedCodeRequest = await this.verificationRequestRepository.GetVerificationRequest(type, userId);

            if (storedCodeRequest == null)
            {
                return false;
            }

            var validatedRequest = storedCodeRequest.ValidateCode(code);

            await this.verificationRequestRepository.UpdateAttemptedRequest(validatedRequest);

            // todo problem+json stuff KYC-36
            return validatedRequest.Status == VerificationRequestStatus.Confirmed;
        }
    }
}
