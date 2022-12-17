using NHASoftware.ConsumableEntities.DTOs;

namespace NHASoftware.Views.ViewModels.SocialVMs
{
    public class HomeVM
    {
        public IEnumerable<PostDTO> SocialPosts { get; set; }

        public HomeVM(IEnumerable<PostDTO> posts)
        {
            SocialPosts = posts;
        }
    }
}
