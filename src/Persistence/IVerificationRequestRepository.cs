using System.Threading.Tasks;

namespace Apollo.Persistence
{
    public interface IVerificationRequestRepository
    {
        /// <summary>
        /// Add new verification request
        /// </summary>
        /// <param name="dto">Verification request details</param>
        /// <returns></returns>
        Task<VerificationRequestDto> AddAsync(VerificationRequestDto dto);

        /// <summary>
        /// Get existing verification request
        /// </summary>
        /// <param name="lykkeUserId">Lykke User Id</param>
        /// <param name="requestId">Verification Request Id</param>
        /// <returns></returns>
        Task<VerificationRequestDto> GetAsync(string lykkeUserId, string requestId);

        /// <summary>
        /// Update existing verification request
        /// </summary>
        /// <param name="dto">Verification request update details</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException">Thrown when there is not verification request in database</exception>
        Task UpdateAsync(VerificationRequestDto dto);
    }
}
