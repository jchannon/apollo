// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Apollo.Features.Verification;
    using Apollo.Persistence;

    public class AzureInMemoryRepository : IVerificationRequestRepository
    {
        private static readonly Dictionary<(string userId, VerificationType type), VerificationRequest> InMemDataStore = new Dictionary<(string userId, VerificationType type), VerificationRequest>();

        public Task StoreNewVerificationRequest(VerificationRequest verificationRequest)
        {
            InMemDataStore[(verificationRequest.UserId, verificationRequest.VerificationType)] = verificationRequest;
            return Task.CompletedTask;
        }

        public Task<VerificationRequest> GetVerificationRequest(VerificationType type, string userId)
        {
            if (InMemDataStore.ContainsKey((userId, type)))
            {
                return Task.FromResult(InMemDataStore[(userId, type)]);
            }

            return Task.FromResult<VerificationRequest>(null);
        }

        public Task UpdateAttemptedRequest(VerificationRequest storedCodeRequest)
        {
            if (InMemDataStore.ContainsKey((storedCodeRequest.UserId, storedCodeRequest.VerificationType)))
            {
                InMemDataStore[(storedCodeRequest.UserId, storedCodeRequest.VerificationType)] = storedCodeRequest;
            }

            return Task.CompletedTask;
        }
    }
}
