using NHASoftware.Models.ForumModels;

namespace NHASoftware.ViewModels
{
    public class ForumTopicDetailsView
    {
        public ForumTopic topic { get; set; }
        public List<ForumPost> Posts { get; set; }
    }
}
