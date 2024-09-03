using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Functions.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _blobContainerName = "treasurehunt";

        public BlobStorageService(string conn)
        {
            _blobServiceClient = new BlobServiceClient(conn);
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteBlobAsync(string blobUri)
        {
            Uri uri = new Uri(blobUri);
            string blobName = uri.Segments[^1];
            var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

    }
}
