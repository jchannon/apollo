// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Persistence.AzureStorage
{
    using System;
    using System.Threading.Tasks;
    using Apollo.Features.Verification;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class VerificationRequestRepository : IVerificationRequestRepository
    {
        private const string TableName = "VerificationRequests";

        private readonly CloudStorageAccount cloudStorageAccount;

        public VerificationRequestRepository(
            string connectionString)
        {
            this.cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task StoreNewVerificationRequest(VerificationRequest verificationRequest)
        {
            var entity = new VerificationRequestEntity(verificationRequest);

            var table = await this.GetTableAsync();

            await table.ExecuteAsync(TableOperation.InsertOrReplace(entity));
        }

        public async Task<VerificationRequest> GetVerificationRequest(VerificationType type, string userId)
        {
            var table = await this.GetTableAsync();

            var tableResult = await table.ExecuteAsync(TableOperation.Retrieve<VerificationRequestEntity>(userId, type.ToString()));

            if (tableResult.HttpStatusCode != 200)
            {
                return null;
            }

            var result = (VerificationRequestEntity)tableResult.Result;

            return new VerificationRequest(
                result.UserId,
                Enum.Parse<VerificationType>(result.VerificationType),
                (VerificationRequestStatus)result.Status,
                result.ExpirationDate,
                new VerificationCode(result.Code),
                result.Attempts);
        }

        public async Task UpdateAttemptedRequest(VerificationRequest storedCodeRequest)
        {
            var table = await this.GetTableAsync();

            var existingEntry = await table.ExecuteAsync(TableOperation.Retrieve(storedCodeRequest.UserId, storedCodeRequest.VerificationType.ToString()));

            if (existingEntry == null)
            {
                throw new EntityNotFoundException();
            }

            var entity = new VerificationRequestEntity(storedCodeRequest)
            {
                ETag = existingEntry.Etag,
            };

            await table.ExecuteAsync(TableOperation.Replace(entity));
        }

        private async Task<CloudTable> GetTableAsync()
        {
            var cloudTableClient = this.cloudStorageAccount.CreateCloudTableClient();

            var cloudTable = cloudTableClient.GetTableReference(TableName);

            // todo CreateIfNotExists is slow, move it out to startup.
            await cloudTable.CreateIfNotExistsAsync();

            return cloudTable;
        }
    }
}
