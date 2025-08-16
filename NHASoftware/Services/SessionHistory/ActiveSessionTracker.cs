using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Entities.Session;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.SessionHistory
{
    public class ActiveSessionTracker : IActiveSessionTracker
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ActiveSessionTracker> _logger;

        public ActiveSessionTracker(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ILogger<ActiveSessionTracker> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> CreateLoginEvent(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                await _unitOfWork.SessionHistoryRepository.AddAsync(new SessionHistoryEvent()
                {
                    LoginEventDescription = SessionEvents.Login,
                    Time = DateTime.UtcNow,
                    userId = user.Id
                });

                var numberOfChangesToDB = await _unitOfWork.CompleteAsync();

                _logger.LogTrace($"Attempting to create session history event for user - {user.DisplayName} # of changes to DB - {numberOfChangesToDB}");
                return numberOfChangesToDB > 0;
            }

            return false;
        }

        public async Task<bool> CreateLogoutEvent(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                await _unitOfWork.SessionHistoryRepository.AddAsync(new SessionHistoryEvent()
                {
                    LoginEventDescription = SessionEvents.Logout,
                    Time = DateTime.UtcNow,
                    userId = user.Id
                });

                var numberOfChangesToDB = await _unitOfWork.CompleteAsync();

                _logger.LogTrace($"Attempting to create session history event for user - {user.DisplayName} # of changes to DB - {numberOfChangesToDB} - event - {SessionEvents.Logout}");
                return numberOfChangesToDB > 0;
            }

            return false;
        }

        public async Task<bool> CreateRenewEvent(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                await _unitOfWork.SessionHistoryRepository.AddAsync(new SessionHistoryEvent()
                {
                    LoginEventDescription = SessionEvents.RenewActive,
                    Time = DateTime.UtcNow,
                    userId = user.Id
                });

                var numberOfChangesToDB = await _unitOfWork.CompleteAsync();

                _logger.LogTrace($"Attempting to create session history event for user - {user.DisplayName} # of changes to DB - {numberOfChangesToDB} - event - {SessionEvents.RenewActive}");
                return numberOfChangesToDB > 0;
            }

            return false;
        }

        public async Task<DateTime?> GetUserLastActiveTime(ApplicationUser user)
        {
            var userSessionActivitySorted = await _unitOfWork.SessionHistoryRepository.GetSortedSessionActivityForUserAsync(user.Id);

            if(userSessionActivitySorted.Any())
                return userSessionActivitySorted.First().Time;

            return null;
        }
    }
}
