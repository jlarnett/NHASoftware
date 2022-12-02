using NHASoftware.Entities.Forums;

namespace NHASoftware.ViewModels
{
    public class ForumPostDetailModel
    {
        public ForumPost ForumPost { get; set; }
        public List<ForumComment> ForumComments { get; set; }
    }
}
