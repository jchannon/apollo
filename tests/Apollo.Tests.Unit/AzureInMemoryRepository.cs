namespace Apollo.Tests.Unit
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Apollo.Features.Verification;
    using Apollo.Persistence;

    public class AzureInMemoryRepository : IVerificationRequestRepository
    {
        private static readonly Dictionary<(string userId, VerificationType type), VerificationRequest> inMemDataStore = new Dictionary<(string userId, VerificationType type), VerificationRequest>();

        public Task StoreNewVerificationRequest(VerificationRequest verificationRequest)
        {
            inMemDataStore[(verificationRequest.UserId, verificationRequest.VerificationType)] = verificationRequest;
            return Task.CompletedTask;
        }

        public Task<VerificationRequest> GetVerificationRequest(VerificationType type, string userId)
        {
            if (inMemDataStore.ContainsKey((userId, type)))
            {
                return Task.FromResult(inMemDataStore[(userId, type)]);
            }

            return Task.FromResult<VerificationRequest>(null);
        }

        public Task UpdateAttemptedRequest(VerificationRequest storedCodeRequest)
        {
            if (inMemDataStore.ContainsKey((storedCodeRequest.UserId, storedCodeRequest.VerificationType)))
            {
                inMemDataStore[(storedCodeRequest.UserId, storedCodeRequest.VerificationType)] = storedCodeRequest;
            }

            return Task.CompletedTask;
        }
    }
}
