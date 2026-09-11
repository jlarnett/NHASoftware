using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.Services.ProfilePicture;

namespace NHA.Website.Software.Controllers;

[Route("ProfilePictures")]
public class ProfilePicturesController : Controller
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IProfilePictureStorage _profilePictureStorage;

    public ProfilePicturesController(IWebHostEnvironment hostEnvironment, IProfilePictureStorage profilePictureStorage)
    {
        _hostEnvironment = hostEnvironment;
        _profilePictureStorage = profilePictureStorage;
    }

    [HttpGet("{*profilePicturePath}")]
    public async Task<IActionResult> Get(string? profilePicturePath)
    {
        if (string.IsNullOrWhiteSpace(profilePicturePath))
        {
            return NotFound();
        }

        var safeFileName = Path.GetFileName(profilePicturePath);
        var localFilePath = Path.Combine(_hostEnvironment.WebRootPath, "ProfilePictures", safeFileName);

        if (System.IO.File.Exists(localFilePath))
        {
            return PhysicalFile(localFilePath, GetContentType(safeFileName));
        }

        var blobStream = await _profilePictureStorage.OpenReadAsync(profilePicturePath, HttpContext.RequestAborted);

        if (blobStream != null)
        {
            return File(blobStream, GetContentType(profilePicturePath), enableRangeProcessing: true);
        }

        return NotFound();
    }

    private static string GetContentType(string path)
    {
        return ContentTypeProvider.TryGetContentType(path, out var contentType)
            ? contentType
            : "application/octet-stream";
    }
}
