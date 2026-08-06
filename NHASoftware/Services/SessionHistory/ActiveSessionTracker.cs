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
        private readonly Dictionary<string, DateTime?> _lastActiveTimesCache = new(StringComparer.Ordinal);

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

        public Task<DateTime?> GetUserLastActiveTime(ApplicationUser user)
        {
            return GetUserLastActiveTime(user.Id);
        }

        public async Task<DateTime?> GetUserLastActiveTime(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            if (_lastActiveTimesCache.TryGetValue(userId, out var lastActiveTime))
            {
                return lastActiveTime;
            }

            lastActiveTime = await _unitOfWork.SessionHistoryRepository.GetLastSessionActivityForUserAsync(userId);
            _lastActiveTimesCache[userId] = lastActiveTime;

            return lastActiveTime;
        }

        public async Task<IReadOnlyDictionary<string, DateTime?>> GetUsersLastActiveTimesAsync(IEnumerable<string> userIds)
        {
            var distinctUserIds = userIds
                .Where(userId => !string.IsNullOrWhiteSpace(userId))
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            if (distinctUserIds.Length == 0)
            {
                return new Dictionary<string, DateTime?>();
            }

            var missingUserIds = distinctUserIds
                .Where(userId => !_lastActiveTimesCache.ContainsKey(userId))
                .ToArray();

            if (missingUserIds.Length > 0)
            {
                var fetchedLastActiveTimes = await _unitOfWork.SessionHistoryRepository.GetLastSessionActivityForUsersAsync(missingUserIds);

                foreach (var userId in missingUserIds)
                {
                    _lastActiveTimesCache[userId] = fetchedLastActiveTimes.GetValueOrDefault(userId);
                }
            }

            return distinctUserIds.ToDictionary(userId => userId, userId => _lastActiveTimesCache[userId], StringComparer.Ordinal);
        }
    }
}
