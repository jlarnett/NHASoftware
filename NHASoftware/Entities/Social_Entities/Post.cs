using NHASoftware.Entities.Identity;

namespace NHASoftware.Entities.Social_Entities
{
    public class Post
    {
        public int? Id { get; set; }
        public string Summary { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int? ParentPostId { get; set; }
        public Post? ParentPost { get; set; }
    }
}
