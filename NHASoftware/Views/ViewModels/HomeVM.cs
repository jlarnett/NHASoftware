using NHASoftware.ConsumableEntities.DTOs;

namespace NHASoftware.ViewModels
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
