using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.ChatSystem;
using NHA.Website.Software.Entities.Identity;

namespace NHA.Website.Software.Views.ViewModels.ChatUI
{
    public class ChatUIViewModel
    {
        public ApplicationUser? Friend { get; set; }
        public List<ChatMessageDTO> ChatMessages { get; set; } 


        public ChatUIViewModel(ApplicationUser? friend, List<ChatMessageDTO> messages)
        {
            Friend = friend;
            ChatMessages = messages;
        }
    }
}
