using NHASoftware.Entities.Forums;

namespace NHA.Website.Software.Views.ViewModels.ForumVMs
{
    public class ForumPostDetailModel
    {
        public ForumPost ForumPost { get; set; } = new ForumPost();
        public List<ForumComment> ForumComments { get; set; } = new List<ForumComment>();
    }
}
