using System.ComponentModel.DataAnnotations;
using NHA.Website.Software.Entities.Identity;
namespace NHA.Website.Software.Entities.Social_Entities;
public class Post
{
    public int? Id { get; set; }

    [Required] public string Summary { get; set; } = string.Empty;
    public DateTime? CreationDate { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int? ParentPostId { get; set; }
    public Post? ParentPost { get; set; }
    public bool IsHiddenFromUserProfile { get; set; } = false;
    public bool IsHiddenFromMainContentFeed { get; set; } = false;
    [Required] public bool IsDeletedFlag { get; set; } = false;

    public ICollection<PostImage> PostImages { get; } = new List<PostImage>();
}
