using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;

namespace NHA.Website.Software.Services.ProfilePicture
{
    public class ProfilePictureFileScrubber : IProfilePictureFileScrubber
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfilePictureFileScrubber> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IProfilePictureStorage _profilePictureStorage;

        public ProfilePictureFileScrubber(
            ApplicationDbContext context,
            ILogger<ProfilePictureFileScrubber> logger,
            IWebHostEnvironment hostEnvironment,
            IProfilePictureStorage profilePictureStorage)
        {
            _context = context;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _profilePictureStorage = profilePictureStorage;
        }

        public async Task RemoveOldProfilePicturesFromFolder()
        {
            var removedProfilePicturePaths = _context.RemovedProfilePicturePaths;

            if (removedProfilePicturePaths == null)
            {
                return;
            }

            var paths = await removedProfilePicturePaths.ToListAsync();

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path.Path)
                    || path.Path.Contains("DefaultProfilePicture.png", StringComparison.OrdinalIgnoreCase))
                {
                    removedProfilePicturePaths.Remove(path);
                    continue;
                }

                try
                {
                    var localFilePath = Path.Combine(_hostEnvironment.WebRootPath, "ProfilePictures", Path.GetFileName(path.Path));

                    if (File.Exists(localFilePath))
                    {
                        File.Delete(localFilePath);
                    }
                    else
                    {
                        await _profilePictureStorage.DeleteAsync(path.Path);
                    }

                    removedProfilePicturePaths.Remove(path);
                }
                catch (Exception ex)
                {
                    _logger.LogTrace(ex, "Was unable to delete profile picture from local or blob storage.");
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
