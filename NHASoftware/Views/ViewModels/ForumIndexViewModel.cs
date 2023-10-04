using NHASoftware.Entities.Forums;

namespace NHASoftware.ViewModels
{
    public class ForumIndexViewModel
    {
        public IEnumerable<ForumSection> ForumSections { get; set; } = Enumerable.Empty<ForumSection>();
        public IEnumerable<ForumTopic> ForumTopics { get; set; } = Enumerable.Empty<ForumTopic>();
        public List<KeyValuePair<int, ForumTopic>> Forums { get; set; } = new List<KeyValuePair<int, ForumTopic>>();

    }
}
