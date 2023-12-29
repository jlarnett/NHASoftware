using System.ComponentModel.DataAnnotations;
using NHA.Website.Software.Entities.Identity;

namespace NHA.Website.Software.Entities.ChatSystem
{
    public class ChatMessage
    {
        public int? Id { get; set; }
        [Required]public string? SenderUserId { get; set; }
        public ApplicationUser? SenderUser { get; set; }

        [Required]public string? RecipientUserId { get; set; }
        public ApplicationUser? RecipientUser { get; set; }

        [Required] public string Summary { get; set; } = string.Empty;

        public DateTime? CreationDate { get; set; }

        public bool MessageViewedByRecipient { get; set; }
    }
}
