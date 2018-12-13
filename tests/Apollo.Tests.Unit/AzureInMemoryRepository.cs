// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Apollo.Features.Verification;
    using Apollo.Persistence;

    public class AzureInMemoryRepository : IVerificationRequestRepository
    {
        private static readonly ConcurrentDictionary<(string userId, VerificationType type), VerificationRequest> InMemDataStore = new ConcurrentDictionary<(string userId, VerificationType type), VerificationRequest>();

        public Task StoreNewVerificationRequest(VerificationRequest verificationRequest)
        {
            InMemDataStore[(verificationRequest.UserId, verificationRequest.VerificationType)] = verificationRequest;
            return Task.CompletedTask;
        }

        public Task<VerificationRequest> GetVerificationRequest(VerificationType type, string userId)
        {
            return InMemDataStore.ContainsKey((userId, type)) ? Task.FromResult(InMemDataStore[(userId, type)]) : Task.FromResult<VerificationRequest>(null);
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
