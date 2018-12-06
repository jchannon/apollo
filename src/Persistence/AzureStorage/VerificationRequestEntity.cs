using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Apollo.Persistence.AzureStorage
{
    public class VerificationRequestEntity : TableEntity
    {
        public string LykkeUserId { get; set; }

        public int Status { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int Subject { get; set; }

        public int Method { get; set; }

        public string Code { get; set; }

        public VerificationRequestEntity()
        {
        }

        public VerificationRequestEntity(VerificationRequestDto src)
        {
            PartitionKey = src.LykkeUserId;
            RowKey = src.Id ?? Guid.NewGuid().ToString("N");
            LykkeUserId = src.LykkeUserId;
            ExpirationDate = src.ExpirationDate;
            Code = src.Code;
            Method = (int) src.Method;
            Status = (int) src.Status;
            Subject = (int) src.Subject;
        }

        public VerificationRequestDto ToDto()
        {
            return new VerificationRequestDto
            {
                Id = RowKey,
                Status = (VerificationRequestStatus) Status,
                Method = (VerificationRequestMethod) Method,
                Subject = (VerificationRequestSubject) Subject,
                ExpirationDate = ExpirationDate,
                Code = Code,
                LykkeUserId = LykkeUserId
            };
        }
    }
}
