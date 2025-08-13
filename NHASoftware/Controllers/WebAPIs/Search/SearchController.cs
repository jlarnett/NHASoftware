using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Controllers.WebAPIs.Search
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{searchString}")]
        public async Task<ActionResult<SearchResponse>> Index(string? searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return BadRequest();

            var animeSearchResults =
                await _unitOfWork.AnimePageRepository.FindAsync(ap => ap.AnimeName.Contains(searchString));
            var gameSearchResults =
                await _unitOfWork.GamePageRepository.FindAsync(ap => ap.Name.Contains(searchString));

            return Ok(new SearchResponse(animeSearchResults, gameSearchResults));
        }
    }
}
