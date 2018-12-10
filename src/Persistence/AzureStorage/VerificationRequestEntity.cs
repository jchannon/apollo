namespace Apollo.Persistence.AzureStorage
{
    using System;
    using Apollo.Features.Verification;
    using Microsoft.WindowsAzure.Storage.Table;

    public class VerificationRequestEntity : TableEntity
    {
        public VerificationRequestEntity()
        {
        }

        public VerificationRequestEntity(VerificationRequest src)
        {
            this.PartitionKey = src.UserId;
            this.RowKey = src.VerificationType.ToString();
            this.Code = src.Code.ToString();
            this.ExpirationDate = src.ExpiryDate;
            this.Status = (int)src.Status;
            this.Attempts = src.Attempts;
        }

        public string UserId => this.PartitionKey;

        public string VerificationType => this.RowKey;

        public int Status { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string Code { get; set; }

        public int Attempts { get; set; }
    }
}
