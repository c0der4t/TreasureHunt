using Azure;
using Azure.Data.Tables;
using Functions.Models;

namespace Functions.Services
{
    public class TableStorageService
    {

        private readonly TableClient _findsTableClient;
        private readonly TableClient _hintsTableClient;

        private readonly string FindPartitionKey = "FindPartition";
        private readonly string HintPartitionKey = "HintPartition";

        public TableStorageService(string conn)
        {
            _findsTableClient = new TableClient(conn, "Finds");
            _hintsTableClient = new TableClient(conn, "Hints");
        }

        public async Task AddFindAsync(Find newFind)
        {
            newFind.PartitionKey = FindPartitionKey;
            newFind.RowKey = Guid.NewGuid().ToString();

            try
            {
                await _findsTableClient.AddEntityAsync(newFind);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException($"Error adding entity to table storage: {ex.Message}");
            }
        }

        public async Task<List<Find>> GetAllFindsAsync()
        {
            var finds = new List<Find>();
            await foreach (var find in _findsTableClient.QueryAsync<Find>())
            {
                finds.Add(find);
            }

            return finds;
        }

        public async Task<List<Hint>> GetAllHintsAsync()
        {
            var hints = new List<Hint>();
            await foreach (var hint in _findsTableClient.QueryAsync<Hint>())
            {
                hints.Add(hint);
            }

            return hints;
        }

    }
}
