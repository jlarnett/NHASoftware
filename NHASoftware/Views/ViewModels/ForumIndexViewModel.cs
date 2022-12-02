using NHASoftware.Entities.Forums;

namespace NHASoftware.ViewModels
{
    public class ForumIndexViewModel
    {
        public IEnumerable<ForumSection> ForumSections { get; set; }
        public IEnumerable<ForumTopic> ForumTopics { get; set; }
        public List<KeyValuePair<int, ForumTopic>> Forums { get; set; }
        
    }
}
