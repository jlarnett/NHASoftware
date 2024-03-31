using NHA.Website.Software.ConsumableEntities.DTOs;

namespace NHA.Website.Software.Views.Shared.ChatSystem.ViewModels
{
    public class ChatUIMultiMessage
    {
        public List<ChatMessageDTO> ChatMessages { get; set; }


        public ChatUIMultiMessage(List<ChatMessageDTO> messages)
        {
            ChatMessages = messages;
        }
    }
}
