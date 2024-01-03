
namespace NHA.Website.Software.Services.ProfilePicture
{
    public interface IProfilePictureFileScrubber
    {
        Task RemoveOldProfilePicturesFromFolder();
    }
}