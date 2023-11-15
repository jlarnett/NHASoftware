using NHA.Website.Software.Entities.Forums;
namespace NHA.Website.Software.Views.ViewModels.ForumVMs;
public class ForumTopicDetailsView
{
    public ForumTopic topic { get; set; } = new ForumTopic();
    public List<ForumPost> Posts { get; set; } = new List<ForumPost>();
}
