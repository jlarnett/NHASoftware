using NHA.Website.Software.Entities.ChatSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.ChatSystem;
public interface IChatMessageRepository : IGenericRepository<ChatMessage>
{
    Task<List<ChatMessage>> GetChatMessagesAsync(string senderUserId, string recipientUserId);
    Task<List<ChatMessage>> GetNewMessageBetweenUsers(string senderUserId, string recipientUserId);
}
