using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace NHA.Website.Software.Services.Social;

public interface IPostVideoStorage
{
    Task<string> SaveAsync(IFormFile mediaFile, CancellationToken cancellationToken = default);
    Task<Stream?> OpenReadAsync(string mediaPath, CancellationToken cancellationToken = default);
    Task DeleteAsync(string mediaPath, CancellationToken cancellationToken = default);
}

public sealed class PostVideoStorageOptions
{
    public string? ConnectionString { get; set; }
    public string? ContainerName { get; set; }
}

public sealed class AzureBlobPostVideoStorage(IOptions<PostVideoStorageOptions> options) : IPostVideoStorage
{
    private const string PostMediaFolderName = "PostMedia";
    private readonly PostVideoStorageOptions _options = options.Value;

    public async Task<string> SaveAsync(IFormFile mediaFile, CancellationToken cancellationToken = default)
    {
        var containerClient = GetRequiredContainerClient();
        var fileExtension = Path.GetExtension(mediaFile.FileName).ToLowerInvariant();
        var mediaPath = $"{PostMediaFolderName}/{Guid.NewGuid()}{fileExtension}";
        var blobClient = containerClient.GetBlobClient(mediaPath);

        await using var stream = mediaFile.OpenReadStream();
        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = string.IsNullOrWhiteSpace(mediaFile.ContentType)
                        ? "application/octet-stream"
                        : mediaFile.ContentType
                }
            },
            cancellationToken);

        return mediaPath;
    }

    public async Task<Stream?> OpenReadAsync(string mediaPath, CancellationToken cancellationToken = default)
    {
        var containerClient = GetOptionalContainerClient();

        if (containerClient == null || string.IsNullOrWhiteSpace(mediaPath))
        {
            return null;
        }

        try
        {
            return await containerClient.GetBlobClient(NormalizeMediaPath(mediaPath))
                .OpenReadAsync(cancellationToken: cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string mediaPath, CancellationToken cancellationToken = default)
    {
        var containerClient = GetOptionalContainerClient();

        if (containerClient == null || string.IsNullOrWhiteSpace(mediaPath))
        {
            return;
        }

        try
        {
            await containerClient.GetBlobClient(NormalizeMediaPath(mediaPath))
                .DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
        }
    }

    private BlobContainerClient GetRequiredContainerClient()
    {
        return GetOptionalContainerClient()
            ?? throw new InvalidOperationException("Azure blob storage is not configured for post videos.");
    }

    private BlobContainerClient? GetOptionalContainerClient()
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString)
            || string.IsNullOrWhiteSpace(_options.ContainerName))
        {
            return null;
        }

        return new BlobContainerClient(_options.ConnectionString, _options.ContainerName);
    }

    private static string NormalizeMediaPath(string mediaPath)
    {
        return mediaPath.Replace('\\', '/');
    }
}
