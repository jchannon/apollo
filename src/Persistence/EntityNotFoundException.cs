using System;
using System.Runtime.Serialization;

namespace Apollo.Persistence
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string partitionKey, string rowKey, string message = null) : base(message ?? "Entity not found")
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
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
