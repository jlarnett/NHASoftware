using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.ChatSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.ChatSystem
{
    public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ChatMessage>> GetChatMessagesAsync(string senderUserId, string recipientUserId)
        {
            return await _context.ChatMessages!.Include(cm => cm.SenderUser).Include(cm => cm.RecipientUser)
                .Where(cm => (cm.SenderUserId!.Equals(senderUserId) && cm.RecipientUserId!.Equals(recipientUserId) ) || (cm.RecipientUserId!.Equals(senderUserId)) && cm.SenderUserId.Equals(recipientUserId) ).OrderBy(cm => cm.CreationDate).ToListAsync();
        }

        public async Task<List<ChatMessage>> GetNewMessageBetweenUsersAsync(string senderUserId, string recipientUserId)
        {
            var result = (await _context.ChatMessages!.Include(cm => cm.SenderUser).Include(cm => cm.RecipientUser)
                .Where(cm =>
                    cm.MessageViewedByRecipient.Equals(false) && cm.RecipientUserId!.Equals(recipientUserId) &&
                    cm.SenderUserId!.Equals(senderUserId)).ToListAsync());

            return result;
        }
    }
}
