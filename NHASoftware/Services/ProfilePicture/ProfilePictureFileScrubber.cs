using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;

namespace NHA.Website.Software.Services.ProfilePicture
{
    public class ProfilePictureFileScrubber : IProfilePictureFileScrubber
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfilePictureFileScrubber> _logger;

        public ProfilePictureFileScrubber(ApplicationDbContext context, ILogger<ProfilePictureFileScrubber> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task RemoveOldProfilePicturesFromFolder()
        {
            var paths = await _context.RemovedProfilePicturePaths!.ToListAsync();

            foreach (var path in paths)
            {
                if (File.Exists(path.Path) && !path.Path.Contains("DefaultProfilePicture.png"))
                {
                    try
                    {
                        File.Delete(path.Path);
                        _context.RemovedProfilePicturePaths!.Remove(path);
                    }
                    catch (Exception e)
                    {
                        _logger.LogTrace("Was unable to delete profile picture file from file system still in use");
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
