using Azure;
using Azure.Data.Tables;

namespace Functions.Models
{
    public class Find : ITableEntity
    {
        public string FindName { get; set; }
        public string FinderName { get; set; }
        public string ImageURI { get; set; }

        public Find(string findName, string finderName)
        {
            FindName = findName;
            FinderName = finderName;
        }

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
