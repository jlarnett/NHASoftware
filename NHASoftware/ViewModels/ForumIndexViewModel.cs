using NHASoftware.Models.ForumModels;

namespace NHASoftware.ViewModels
{
    public class ForumIndexViewModel
    {
        public List<ForumSection> ForumSections { get; set; }
        public List<ForumTopic> ForumTopics { get; set; }

        public List<KeyValuePair<int, ForumTopic>> Forums { get; set; }
        
    }
}
