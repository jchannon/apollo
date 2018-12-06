using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Apollo.Persistence.AzureStorage
{
    public class VerificationRequestRepository : IVerificationRequestRepository
    {
        private readonly CloudStorageAccount _cloudStorageAccount;
        private const string TableName = "VerificationRequests";

        public VerificationRequestRepository(
            string connectionString)
        {
            _cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task<VerificationRequestDto> AddAsync(VerificationRequestDto dto)
        {
            var entity = new VerificationRequestEntity(dto);

            var table = await GetTableAsync();

            await table.ExecuteAsync(TableOperation.Insert(entity));

            return entity.ToDto();
        }

        public async Task<VerificationRequestDto> GetAsync(string lykkeUserId, string requestId)
        {
            var table = await GetTableAsync();

            var tableResult =
                await table.ExecuteAsync(TableOperation.Retrieve<VerificationRequestEntity>(lykkeUserId, requestId));

            return ((VerificationRequestEntity) tableResult.Result)?.ToDto();
        }

        public async Task UpdateAsync(VerificationRequestDto dto)
        {
            var table = await GetTableAsync();

            var existingEntity =
                await table.ExecuteAsync(TableOperation.Retrieve<VerificationRequestEntity>(dto.LykkeUserId, dto.Id));

            if (existingEntity == null)
                throw new EntityNotFoundException(dto.LykkeUserId, dto.Id);

            var entity = new VerificationRequestEntity(dto)
            {
                ETag = existingEntity.Etag
            };

            await table.ExecuteAsync(TableOperation.Merge(entity));
        }

        private async Task<CloudTable> GetTableAsync()
        {
            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient();

            var cloudTable = cloudTableClient.GetTableReference(TableName);

            await cloudTable.CreateIfNotExistsAsync();

            return cloudTable;
        }
    }
}
