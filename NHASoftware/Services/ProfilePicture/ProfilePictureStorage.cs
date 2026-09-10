using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace NHA.Website.Software.Services.ProfilePicture;

public interface IProfilePictureStorage
{
    Task<string> SaveAsync(IFormFile profilePicture, CancellationToken cancellationToken = default);
    Task<Stream?> OpenReadAsync(string profilePicturePath, CancellationToken cancellationToken = default);
    Task DeleteAsync(string profilePicturePath, CancellationToken cancellationToken = default);
}

public sealed class ProfilePictureStorageOptions
{
    public string? ConnectionString { get; set; }
    public string? ContainerName { get; set; }
}

public sealed class AzureBlobProfilePictureStorage(IOptions<ProfilePictureStorageOptions> options) : IProfilePictureStorage
{
    private const string ProfilePicturesFolderName = "ProfilePictures";
    private readonly ProfilePictureStorageOptions _options = options.Value;

    public async Task<string> SaveAsync(IFormFile profilePicture, CancellationToken cancellationToken = default)
    {
        var containerClient = GetRequiredContainerClient();
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var fileExtension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();
        var profilePictureFileName = $"{Guid.NewGuid()}{fileExtension}";
        var blobClient = containerClient.GetBlobClient(BuildBlobPath(profilePictureFileName));

        await using var stream = profilePicture.OpenReadStream();
        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = string.IsNullOrWhiteSpace(profilePicture.ContentType)
                        ? "application/octet-stream"
                        : profilePicture.ContentType
                }
            },
            cancellationToken);

        return profilePictureFileName;
    }

    public async Task<Stream?> OpenReadAsync(string profilePicturePath, CancellationToken cancellationToken = default)
    {
        var containerClient = GetOptionalContainerClient();

        if (containerClient == null || string.IsNullOrWhiteSpace(profilePicturePath))
        {
            return null;
        }

        try
        {
            return await containerClient.GetBlobClient(BuildBlobPath(profilePicturePath))
                .OpenReadAsync(cancellationToken: cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string profilePicturePath, CancellationToken cancellationToken = default)
    {
        var containerClient = GetOptionalContainerClient();

        if (containerClient == null || string.IsNullOrWhiteSpace(profilePicturePath))
        {
            return;
        }

        try
        {
            await containerClient.GetBlobClient(BuildBlobPath(profilePicturePath))
                .DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
        }
    }

    private BlobContainerClient GetRequiredContainerClient()
    {
        return GetOptionalContainerClient()
            ?? throw new InvalidOperationException("Azure blob storage is not configured for profile pictures.");
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

    private static string BuildBlobPath(string profilePicturePath)
    {
        var normalizedPath = profilePicturePath.Replace('\\', '/').TrimStart('/');

        if (normalizedPath.StartsWith($"{ProfilePicturesFolderName}/", StringComparison.OrdinalIgnoreCase))
        {
            return normalizedPath;
        }

        return $"{ProfilePicturesFolderName}/{normalizedPath}";
    }
}
