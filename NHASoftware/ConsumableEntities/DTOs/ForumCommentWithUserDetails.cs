using NHA.Website.Software.Entities.Forums;

namespace NHA.Website.Software.ConsumableEntities.DTOs
{
    public class ForumCommentWithUserDetails
    {
        public required ForumComment Comment { get; set; }
        public int totalUserLikes { get; set; }
        public int totalUserMessages { get; set; }

    }
}
