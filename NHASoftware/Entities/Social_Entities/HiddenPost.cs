using System.ComponentModel.DataAnnotations;
namespace NHA.Website.Software.Entities.Social_Entities;
public class HiddenPost
{
    public int Id { get; set; }

    [Required]
    [MaxLength(36)]
    public string SessionId { get; set; } = string.Empty;

    [Required]
    public int PostId { get; set; }
    public Post? Post { get; set; }
}
