namespace Apollo.Persistence
{
    using System;
    using System.Runtime.Serialization;

    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string partitionKey, string rowKey, string message = null) : base(message ?? "Entity not found")
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }
    }
}
