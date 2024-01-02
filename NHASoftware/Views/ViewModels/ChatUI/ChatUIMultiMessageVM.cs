using NHA.Website.Software.ConsumableEntities.DTOs;

namespace NHA.Website.Software.Views.ViewModels.ChatUI
{
    public class ChatUIMultiMessageVM
    {
        public List<ChatMessageDTO> ChatMessages { get; set; }


        public ChatUIMultiMessageVM(List<ChatMessageDTO> messages)
        {
            ChatMessages = messages;
        }
    }
}
