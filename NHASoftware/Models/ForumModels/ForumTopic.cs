namespace NHASoftware.Models.ForumModels
{
    public class ForumTopic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ForumSection? ForumSection { get; set; }
        public int ForumSectionId { get; set; }
    }
}
