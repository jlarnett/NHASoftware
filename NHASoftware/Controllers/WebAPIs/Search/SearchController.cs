using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using Microsoft.Extensions.Caching.Memory;
using NHA.Website.Software.Caching;

namespace NHA.Website.Software.Controllers.WebAPIs.Search
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _searchCache;

        public SearchController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _searchCache = cache;
        }

        /// <summary>
        /// Returns a list of all possible entities that match the searchString.
        /// Used for site-wide search. Caching Enabled
        /// </summary>
        /// <param name="searchString">string value to search for</param>
        /// <returns></returns>
        [HttpGet("{searchString}")]
        public async Task<ActionResult<SearchResponse>> Index(string? searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return BadRequest();

            // Make the cache key unique per search
            var cacheKey = $"{CachingKeys.Searches}_{searchString.ToLower()}";

            if (!_searchCache.TryGetValue(cacheKey, out SearchResponse? searchResponse))
            {
                // Set cache options
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(4))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                var animeSearchResults =
                    await _unitOfWork.AnimePageRepository.FindWithoutTrackingAsync(ap => ap.AnimeName.Contains(searchString));
                var gameSearchResults =
                    await _unitOfWork.GamePageRepository.FindWithoutTrackingAsync(ap => ap.Name.Contains(searchString));
                var userSearchResults =
                    await _userManager.Users
                        .AsNoTracking()
                        .Where(u => (u.DisplayName != null && u.DisplayName.Contains(searchString)) || u.Email != null && u.Email.Contains(searchString))
                        .ToListAsync();
                searchResponse = new SearchResponse(animeSearchResults, gameSearchResults, userSearchResults);
                _searchCache.Set(cacheKey, searchResponse, cacheOptions);

            }

            return Ok(searchResponse);
        }
    }
}
