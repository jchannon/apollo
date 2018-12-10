namespace Apollo.Tests.Unit
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Apollo.Features.Verification;
    using Apollo.Persistence;

    public class AzureInMemoryRepository : IVerificationRequestRepository
    {
        private Dictionary<(string userId, VerificationType type), VerificationRequest> inMemDataStore = new Dictionary<(string userId, VerificationType type), VerificationRequest>();

        public Task StoreNewVerificationRequest(VerificationRequest verificationRequest)
        {
            this.inMemDataStore[(verificationRequest.UserId, verificationRequest.VerificationType)] = verificationRequest;
            return Task.CompletedTask;
        }

        public Task<VerificationRequest> GetVerificationRequest(VerificationType type, string userId)
        {
            if (this.inMemDataStore.ContainsKey((userId, type)))
            {
                return Task.FromResult(this.inMemDataStore[(userId, type)]);
            }

            return Task.FromResult<VerificationRequest>(null);
        }

        public Task UpdateAttemptedRequest(VerificationRequest storedCodeRequest)
        {
            if (this.inMemDataStore.ContainsKey((storedCodeRequest.UserId, storedCodeRequest.VerificationType)))
            {
                this.inMemDataStore[(storedCodeRequest.UserId, storedCodeRequest.VerificationType)] = storedCodeRequest;
            }

            return Task.CompletedTask;
        }
    }
}
