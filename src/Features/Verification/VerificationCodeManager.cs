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

        public async Task GenerateCode(VerificationType type, string userId, Func<VerificationCode, Task> onSuccessfulGeneration)
        {
            var outstandingRequest = await this.verificationRequestRepository.GetVerificationRequest(type, userId);
            if (outstandingRequest != null && outstandingRequest.IsActive())
            {
                this.logger.LogInformation("User {userId} tried to request an active code when one was already available", userId);
                throw new VerificationAlreadyStarted();
            }

            var verificationRequest = VerificationRequest.GenerateNewVerificationRequest(userId, type, VerificationCode.Generate());

            await this.verificationRequestRepository.StoreNewVerificationRequest(verificationRequest); // todo config the time KYC-43

            await onSuccessfulGeneration(verificationRequest.Code);
        }

        public async Task VerifyCode(VerificationType type, string userId, VerificationCode code)
        {
            var storedCodeRequest = await this.verificationRequestRepository.GetVerificationRequest(type, userId);

            if (storedCodeRequest == null)
            {
                this.logger.LogWarning("User ({userId}) tried to verify a code that hasn't been requested", userId);
                throw new VerificationRequestMissing();
            }

            var validatedRequest = storedCodeRequest.ValidateCode(code);

            await this.verificationRequestRepository.UpdateAttemptedRequest(validatedRequest);

            switch (validatedRequest.Status)
            {
                case VerificationRequestStatus.Pending:
                    throw new VerificationCodeMismatch();
                case VerificationRequestStatus.Expired:
                    throw new VerificationCodeHasExpired();
                case VerificationRequestStatus.Failed:
                    throw new MaximumVerificationAttemptsReached();
            }
        }
    }
}
