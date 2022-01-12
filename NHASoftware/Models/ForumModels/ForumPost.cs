namespace NHASoftware.Models.ForumModels
{
    public class ForumPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ForumText { get; set; }
        public DateTime CreationDate { get; set; }
        public int CommentCount { get; set; }
        public ApplicationUser? User { get; set; }
        public string? UserId { get; set; }
        public ForumTopic? ForumTopic { get; set; }
        public int ForumTopicId { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
    }
}
