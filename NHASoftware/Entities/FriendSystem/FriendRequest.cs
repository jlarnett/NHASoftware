using NHASoftware.Entities.Identity;

namespace NHASoftware.Entities.FriendSystem
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public string SenderUserId { get; set; }
        public ApplicationUser? SenderUser { get; set; }
        public string RecipientUserId { get; set; }
        public ApplicationUser? RecipientUser { get; set; }
        public string Status { get; set; }
    }
}
