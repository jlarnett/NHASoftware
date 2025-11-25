using NHA.Website.Software.Entities.Identity;

namespace NHA.Website.Software.Entities.Forums;
public class ForumComment
{
    public int Id { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public ApplicationUser? User { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ForumPost? ForumPost { get; set; }
    public int ForumPostId { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }

    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
}
