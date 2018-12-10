namespace Apollo.Persistence
{
    using System.Threading.Tasks;
    using Apollo.Features.Verification;

    public interface IVerificationRequestRepository
    {
        Task StoreNewVerificationRequest(VerificationRequest verificationRequest);

        Task<VerificationRequest> GetVerificationRequest(VerificationType type, string userId);

        Task UpdateAttemptedRequest(VerificationRequest storedCodeRequest);
    }
}
