using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.ChatSystem;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.ChatSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Views.Shared.ChatSystem.ViewModels;

namespace NHA.Website.Software.Controllers
{
    public class ChatController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public ChatController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage([Bind("SenderUserId,RecipientUserId,Summary")] ChatMessage chatMessage)
        {
            chatMessage.CreationDate = DateTime.UtcNow;
            chatMessage.MessageViewedByRecipient = false;

            if (ModelState.IsValid)
            {
                _unitOfWork.ChatMessageRepository.Add(chatMessage);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    var message = (await _unitOfWork.ChatMessageRepository.FindAsync(cm =>
                        cm.CreationDate.Equals(chatMessage.CreationDate) &&
                        cm.RecipientUserId!.Equals(chatMessage.RecipientUserId) &&
                        cm.SenderUserId!.Equals(chatMessage.SenderUserId))).First();


                    ModelState.Clear();
                    return PartialView("ChatSystem/_ChatUIMessage", _mapper.Map<ChatMessageDTO>(message));
                }
            }

            return BadRequest();
        }

        [HttpGet("Chat/GetNewMessagesForUser")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> NewMessages()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            IEnumerable<ChatMessage> newMessages = new List<ChatMessage>();

            if (currentUser != null)
            {
                newMessages =  await _unitOfWork.ChatMessageRepository.FindAsync(cm =>
                    cm.MessageViewedByRecipient.Equals(false) && cm.RecipientUserId!.Equals(currentUser.Id));

                return new JsonResult(new { messages = newMessages, success = true});
            }

            return new JsonResult(new {messages = newMessages, success = false});
        }

        [HttpGet("Chat/NewChatsBetweenUsers/{senderUserId}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetNewChatsBetweenUsers(string? senderUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && senderUserId != null)
            {
                var newMessages =
                    await _unitOfWork.ChatMessageRepository.GetNewMessageBetweenUsersAsync(senderUserId, currentUser.Id);
                var newMessageDTOs = newMessages.Select(_mapper.Map<ChatMessage, ChatMessageDTO>).ToList();
                await UpdateChatMessageToSeen(newMessages);

                return PartialView("ChatSystem/_ChatUIMultiMessage", new ChatUIMultiMessage(newMessageDTOs));
            }

            return BadRequest();
        }

        private async Task<bool> UpdateChatMessageToSeen(List<ChatMessage> chatMessages)
        {
            foreach (var chat in chatMessages)
            {
                if (!chat.MessageViewedByRecipient && _userManager.GetUserId(User)!.Equals(chat.RecipientUserId))
                {
                    chat.MessageViewedByRecipient = true;
                    _unitOfWork.ChatMessageRepository.Update(chat);
                }
            }
            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }
    }
}
