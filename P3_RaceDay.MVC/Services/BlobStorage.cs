
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace P3_RaceDay.MVC.Services
{
    public class BlobStorageService
    {
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Please select an image.");
            }

            string connectionString =
                _configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string is missing.");

            string containerName =
                _configuration["AzureStorage:ContainerName"]
                ?? "raceday-images";

            BlobServiceClient blobServiceClient =
                new BlobServiceClient(connectionString);

            BlobContainerClient containerClient =
                blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();

            string fileName =
                $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            BlobClient blobClient =
                containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }
    }
}
