// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification
{
    using System;
    using System.Threading.Tasks;
    using Apollo.Persistence;
    using Microsoft.Extensions.Logging;

    public class VerificationCodeManager
    {
        private readonly IVerificationRequestRepository verificationRequestRepository;

        private readonly ILogger<VerificationCodeManager> logger;

        public VerificationCodeManager(IVerificationRequestRepository verificationRequestRepository, ILogger<VerificationCodeManager> logger)
        {
            this.verificationRequestRepository = verificationRequestRepository;
            this.logger = logger;
        }

        public async Task<bool> GenerateCode(VerificationType type, string userId, Func<VerificationCode, Task> onSuccessfulGeneration)
        {
            var outstandingRequest = await this.verificationRequestRepository.GetVerificationRequest(type, userId);
            if (outstandingRequest != null && outstandingRequest.IsActive())
            {
                this.logger.LogInformation("User {userId} tried to request an active code when one was already available", userId);
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
                this.logger.LogWarning("Failed to dispatch verification code to user");
                return false;
            }

            return true;
        }

        public async Task<bool> VerifyCode(VerificationType type, string userId, VerificationCode code)
        {
            var storedCodeRequest = await this.verificationRequestRepository.GetVerificationRequest(type, userId);

            if (storedCodeRequest == null)
            {
                this.logger.LogWarning("User ({userId}) tried to verify a code that hasn't been requested", userId);
                return false;
            }

            var validatedRequest = storedCodeRequest.ValidateCode(code);

            await this.verificationRequestRepository.UpdateAttemptedRequest(validatedRequest);

            // todo problem+json stuff KYC-36
            return validatedRequest.Status == VerificationRequestStatus.Confirmed;
        }
    }
}
