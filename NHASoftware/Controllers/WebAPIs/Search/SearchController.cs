using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using System.Collections.Generic;

namespace NHA.Website.Software.Controllers.WebAPIs.Search
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public SearchController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet("{searchString}")]
        public async Task<ActionResult<SearchResponse>> Index(string? searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return BadRequest();

            var animeSearchResults =
                await _unitOfWork.AnimePageRepository.FindAsync(ap => ap.AnimeName.Contains(searchString));
            var gameSearchResults =
                await _unitOfWork.GamePageRepository.FindAsync(ap => ap.Name.Contains(searchString));
            var userSearchResults =
                await _userManager.Users
                    .Where(u => (u.DisplayName != null && u.DisplayName.Contains(searchString)) || u.Email != null && u.Email.Contains(searchString)).ToListAsync();

            return Ok(new SearchResponse(animeSearchResults, gameSearchResults, userSearchResults));
        }
    }
}
