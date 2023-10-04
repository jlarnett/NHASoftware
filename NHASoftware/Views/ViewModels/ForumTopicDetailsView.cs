using NHASoftware.Entities.Forums;

namespace NHASoftware.ViewModels
{
    public class ForumTopicDetailsView
    {
        public ForumTopic topic { get; set; }  = new ForumTopic();
        public List<ForumPost> Posts { get; set; } = new List<ForumPost>();
    }
}
