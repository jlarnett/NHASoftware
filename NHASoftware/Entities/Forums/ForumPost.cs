using System.ComponentModel;
using NHA.Website.Software.Entities.Identity;
namespace NHA.Website.Software.Entities.Forums;
public class ForumPost
{
    public int Id { get; set; }

    [DisplayName("Post Title")]
    public string Title { get; set; } = string.Empty;

    [DisplayName("Post")] public string ForumText { get; set; } = string.Empty;

    public DateTime CreationDate { get; set; }
    public int CommentCount { get; set; }
    public ApplicationUser? User { get; set; }
    public string? UserId { get; set; }
    public ForumTopic? ForumTopic { get; set; }

    [DisplayName("Parent Topic")]
    public int ForumTopicId { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
}
