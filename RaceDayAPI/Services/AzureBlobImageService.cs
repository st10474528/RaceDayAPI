using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDayAPI.Services;

public class AzureBlobImageService : IImageService
{
    private readonly string _connectionString;
    private readonly string _containerName = "event-banners";

    public AzureBlobImageService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("AzureBlobStorage")!;
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        // Create container if it doesn't exist and set public read access
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

        return blobClient.Uri.ToString();
    }
}