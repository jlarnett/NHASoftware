using NHASoftware.Entities.Identity;

namespace NHASoftware.Entities.FriendSystem
{
    public class Friends
    {
        public int Id { get; set; }
        public string FriendOneId { get; set; }
        public ApplicationUser? FriendOne { get; set; }
        public string FriendTwoId { get; set; }
        public ApplicationUser? FriendTwo { get; set; }
    }
}
