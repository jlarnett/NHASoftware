using NHASoftware.Entities.Identity;
using NHASoftware.ConsumableEntities.DTOs;

namespace NHASoftware.Views.ViewModels.SocialVMs
{
    public class ProfileVM
    {
        public ApplicationUser? User { get; set; }
        public IEnumerable<PostDTO> UserPosts { get; set; } = Enumerable.Empty<PostDTO>();
    }
}
