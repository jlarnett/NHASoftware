using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.Identity;

namespace NHASoftware.Views.ViewModels.SocialVMs
{
    public class ProfileVM
    {
        public ApplicationUser? User { get; set; }
        public IEnumerable<PostDTO> UserPosts { get; set; } = Enumerable.Empty<PostDTO>();
    }
}
