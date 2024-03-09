using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Infra.Utility.Vendor
{
    public class BucketManager : IBucketManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BucketManager> _logger;
        private readonly string _className = typeof(BucketManager).Name;

        public BucketManager(IConfiguration configuration,
                             ILogger<BucketManager> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> ArmazenarAudio(byte[] bytes)
        {
            try
            {
                var container = _configuration.GetSection("AzureBlobConfiguration")["AudioContainerName"] ?? string.Empty;
                var strConn = _configuration.GetSection("AzureBlobConfiguration")["ConnectionString"] ?? string.Empty;
                var blobName = _configuration.GetSection("AzureBlobConfiguration")["BlobName"] ?? string.Empty;

                BlobServiceClient blobServiceClient = new BlobServiceClient(strConn);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
                await containerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    await blobClient.UploadAsync(stream, true);
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [ArmazenarAudio] => Exception.: {ex.Message}");
                return null;
            }
        }

        public async Task<string> ArmazenarImagem(byte[] bytes)
        {
            try
            {
                var container = _configuration.GetSection("AzureBlobConfiguration")["ImageContainerName"] ?? string.Empty;
                var strConn = _configuration.GetSection("AzureBlobConfiguration")["ConnectionString"] ?? string.Empty;
                var blobName = _configuration.GetSection("AzureBlobConfiguration")["BlobName"] ?? string.Empty;

                BlobServiceClient blobServiceClient = new BlobServiceClient(strConn);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
                await containerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    await blobClient.UploadAsync(stream, true);
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [ArmazenarAudio] => Exception.: {ex.Message}");
                return null;
            }
        }
    }
}