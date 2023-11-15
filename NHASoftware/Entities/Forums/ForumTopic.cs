namespace NHA.Website.Software.Entities.Forums;
public class ForumTopic
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ThreadCount { get; set; }
    public int PostCount { get; set; }
    public DateTime LastestPost { get; set; }
    public ForumSection? ForumSection { get; set; }
    public int ForumSectionId { get; set; }
}
