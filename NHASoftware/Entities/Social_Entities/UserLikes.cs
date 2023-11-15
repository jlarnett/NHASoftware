using System.ComponentModel.DataAnnotations;
using NHA.Website.Software.Entities.Identity;
namespace NHA.Website.Software.Entities.Social_Entities;
public class UserLikes
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    [Required]
    public int PostId { get; set; }
    public Post? Post { get; set; }

    public bool IsDislike { get; set; } = false;
}
