using NHASoftware.Entities.Identity;

namespace NHASoftware.ConsumableEntities.DTOs
{
    public class FriendRequestDTO
    {
        public int Id { get; set; }
        public string SenderUserId { get; set; }
        public ApplicationUser? SenderUser { get; set; }
        public string RecipientUserId { get; set; }
        public ApplicationUser? RecipientUser { get; set; }
        public string? Status { get; set; }
    }
}
