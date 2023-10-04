using NHASoftware.Entities.Identity;

namespace NHA.Website.Software.Entities.FriendSystem
{
    public class Friends
    {
        public int Id { get; set; }
        public string FriendOneId { get; set; } = string.Empty;
        public ApplicationUser? FriendOne { get; set; }
        public string FriendTwoId { get; set; } = string.Empty;
        public ApplicationUser? FriendTwo { get; set; }
    }
}
