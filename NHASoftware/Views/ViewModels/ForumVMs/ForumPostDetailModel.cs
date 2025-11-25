using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.Forums;
namespace NHA.Website.Software.Views.ViewModels.ForumVMs;
public class ForumPostDetailModel
{
    public ForumPost ForumPost { get; set; } = new ForumPost();
    public List<ForumCommentWithUserDetails> ForumComments { get; set; } = [];
}
