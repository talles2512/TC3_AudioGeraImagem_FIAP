using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace AudioGeraImagemWorker.Infra.Vendor
{
    public class BucketManager : IBucketManager
    {
        private readonly string containerName;
        private readonly string connectionString;

        public BucketManager(IConfiguration configuration)
        {
            containerName = configuration.GetSection("AzureBlobConfiguration")["ContainerName"] ?? string.Empty;
            connectionString = configuration.GetSection("AzureBlobConfiguration")["ConnectionString"] ?? string.Empty;
        }

        private async Task<BlobClient> GetBlobClient(string blobName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return containerClient.GetBlobClient(blobName);
        }

        public async Task<string> ArmazenarObjeto(byte[] bytes, string blobName)
        {
            var blobClient = await GetBlobClient(blobName);

            await blobClient.UploadAsync(BinaryData.FromBytes(bytes));

            return blobClient.Uri.ToString();
        }
    }
}