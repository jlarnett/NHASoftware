using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace NHA.Website.Software.Controllers.WebAPIs.Anime
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimePagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnimePagesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<AnimePage>>> GetAnimePages([FromQuery] int pageNumber)
        {
            const int pageSize = 50;
            var totalItems = await _unitOfWork.AnimePageRepository.CountAsync(c => c.Id != null);
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Fix invalid page numbers
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = 1;

            var anime = await _unitOfWork.AnimePageRepository.GetResultPageAsync(pageNumber, 50);
            return Ok(anime);
        }

        [HttpGet]
        public async Task<ActionResult<AnimePage>> GetAnimePage(int? id)
        {
            if (id == null)
                return BadRequest("Id is null");

            var anime = await _unitOfWork.AnimePageRepository.GetByIdAsync(id);

            if (anime == null)
                return NotFound();

            return Ok(anime);
        }

        [HttpPost]
        [Consumes("application/json")]
        [Authorize]
        public async Task<ActionResult<AnimePage>> Create([FromBody] CreateAnimePageRequest request)
        {
            var animePage = new AnimePage()
            {
                AnimeName = request.AnimeName,
                AnimeEnglishName = request.AnimeEnglishName,
                AnimeJapaneseName = request.AnimeJapaneseName,
                AnimeBackground = request.AnimeBackground,
                AnimeGenres = request.AnimeGenres,
                AnimeImageUrl = request.AnimeImageUrl,
                AnimeSummary = request.AnimeSummary,
                AnimeJikanScore = request.AnimeJikanScore,
                AnimeStatus = request.AnimeStatus,
                TrailerUrl = request.TrailerUrl,
                Platforms = request.Platforms,
                Featured = false,
                DownVotes = 0,
                UpVotes = 0,
                EpisodeCount = request.EpisodeCount
            };

            await _unitOfWork.AnimePageRepository.AddAsync(animePage);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                return Created($"{Request.Path}/{animePage.Id}", animePage);
            }

            return BadRequest("Anime failed to save successfully");
        }

    }

    public class CreateAnimePageRequest()
    {
        [DisplayName("Anime Name")]
        [MaxLength(2500)]
        public string AnimeName { get; set; } = string.Empty;

        [DisplayName("Anime English Name")]
        [MaxLength(2500)]
        public string? AnimeEnglishName { get; set; } = string.Empty;

        [DisplayName("Anime Japanese Name")]
        [MaxLength(2500)]
        public string? AnimeJapaneseName { get; set; } = string.Empty;

        [DisplayName("Anime Summary")]
        public string AnimeSummary { get; set; } = string.Empty;

        [DisplayName("Anime Background")]
        public string AnimeBackground { get; set; } = string.Empty;

        [DisplayName("Anime External Image Url")]
        public string? AnimeImageUrl { get; set; } = string.Empty;

        [DisplayName("Anime Jikan Score")]
        public double? AnimeJikanScore { get; set; } = 0;

        [DisplayName("Anime Airing Status")]
        [MaxLength(200)]
        public string? AnimeStatus { get; set; } = string.Empty;

        [DisplayName("Anime Genres")]
        [MaxLength(200)]
        public string? AnimeGenres { get; set; } = string.Empty;
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }

        [DisplayName("Anime Trailer Url")]
        [MaxLength(200)]
        public string? TrailerUrl { get; set; }

        [DisplayName("Episode Count")]
        public int EpisodeCount { get; set; } = 1;

        [DisplayName("Platforms")]
        [MaxLength(200)]
        public string? Platforms { get; set; } = string.Empty;

    }
}
