using Azure;
using Azure.Data.Tables;

namespace Functions.Models
{
    public class Hint : ITableEntity
    {
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public string getID()
        {
            return this.PartitionKey + this.RowKey;
        }

    }
}
