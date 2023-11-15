using NHA.Website.Software.Entities.Identity;

namespace NHA.Website.Software.ConsumableEntities.DTOs;

public class FriendRequestDTO
{
    public int Id { get; set; }
    public string SenderUserId { get; set; } = string.Empty;
    public ApplicationUser? SenderUser { get; set; }
    public string RecipientUserId { get; set; } = string.Empty;
    public ApplicationUser? RecipientUser { get; set; }
    public string? Status { get; set; }
}
