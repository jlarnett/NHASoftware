using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.IdentityModel.Tokens;
using NHA.Helpers.AlphabetSimplify;
using NHA.Helpers.HtmlStringCleaner;
using NHA.Website.Software.Caching;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Services.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Views.Anime.Vms;
namespace NHA.Website.Software.Controllers.AnimeControllers;

[FeatureGate("AnimeEnabled")]
public class AnimeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHtmlStringCleaner _htmlCleaner;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AnimeController> _logger;


    public AnimeController(ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IHtmlStringCleaner htmlCleaner,
        IMemoryCache memoryCache, ILogger<AnimeController> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _htmlCleaner = htmlCleaner;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        //Try to get anime pages from local cache
        if (!_memoryCache.TryGetValue(CachingKeys.Anime, out IEnumerable<AnimePage>? animePages))
        {
            animePages = await _unitOfWork.AnimePageRepository.GetAllAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(12));

            _memoryCache.Set(CachingKeys.Anime, animePages, cacheEntryOptions);
        }

        return View(new IndexViewModel()
        {
            AnimeList = animePages ?? []
        });
    }

    public async Task<IActionResult> Roll(int pageNumber)
    {
        const int pageSize = 50;
        var vm = new RollViewModel();
        var totalItems = await _unitOfWork.AnimePageRepository.CountAsync(c => c.Id != null);
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        // Fix invalid page numbers
        if (pageNumber < 1) pageNumber = 1;
        if (pageNumber > totalPages) pageNumber = 1;

        // Get the valid page results
        var pages = await _unitOfWork.AnimePageRepository.GetResultPageAsync(pageNumber, 50);

        foreach (var page in pages)
        {
            if (!page.TrailerUrl.IsNullOrEmpty())
                vm.RollAnime.Add(page);
        }

        vm.Page = pageNumber;
        return View(vm);
    }

    public async Task<IActionResult> LetterDetail(int id)
    { 
        var letter = AlphabetDecipher.ConvertNumberToAlphabetLetter(id);

        //Try to get anime pages from local cache
        if (!_memoryCache.TryGetValue(CachingKeys.Anime, out IEnumerable<AnimePage>? animePages))
        {
            animePages = await _unitOfWork.AnimePageRepository.GetAllAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(12));

            _memoryCache.Set(CachingKeys.Anime, animePages, cacheEntryOptions);
        }

        var animeList = new List<AnimePage>();

        //Getting all anime that starts with specific alphabet letter
        foreach (var anime in animePages ?? [])
        {
            if (anime.AnimeName.StartsWith(letter))
            {
                animeList.Add(anime);
            }
        }

        //Sorting the list by alphabetical order.
        var alphabeticallySortedAnimeForLetter = animeList.OrderBy(ap => ap.AnimeName).ToList();

        var vm = new LetterIndexViewModel()
        {
            AlphabetLetter = letter,
            AnimeList = alphabeticallySortedAnimeForLetter
        };

        return View("LetterIndex", vm);
    }

    public async Task<IActionResult> GenreIndex(string genre)
    {
        //Try to get anime pages from local cache
        if (!_memoryCache.TryGetValue(CachingKeys.Anime, out IEnumerable<AnimePage>? animePages))
        {
            animePages = await _unitOfWork.AnimePageRepository.GetAllAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(12));

            _memoryCache.Set(CachingKeys.Anime, animePages, cacheEntryOptions);
        }

        List<AnimePage> animeList = [];

        //Getting all anime that starts with specific alphabet letter
        foreach (var anime in animePages ?? [])
        {
            if(anime.AnimeGenres == null) continue;

            if (anime.AnimeGenres.Contains(genre))
            {
                animeList.Add(anime);
            }
        }

        //Sorting the list by alphabetical order.
        var alphabeticallySortedAnimeList = animeList.OrderBy(ap => ap.AnimeName).ToList();

        var vm = new GenreIndexViewModel()
        {
            Genre = genre,
            AnimeList = alphabeticallySortedAnimeList
        };

        return View("GenreIndex", vm);
    }

    public async Task<IActionResult> PlatformIndex(string platform)
    {
        //Try to get anime pages from local cache
        if (!_memoryCache.TryGetValue(CachingKeys.Anime, out IEnumerable<AnimePage>? animePages))
        {
            animePages = await _unitOfWork.AnimePageRepository.GetAllAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(12));

            _memoryCache.Set(CachingKeys.Anime, animePages, cacheEntryOptions);
        }

        List<AnimePage> animeList = [];

        //Getting all anime that starts with specific alphabet letter
        foreach (var anime in animePages ?? [])
        {
            if (anime.Platforms == null) continue;

            if (anime.Platforms.Contains(platform))
            {
                animeList.Add(anime);
            }
        }

        //Sorting the list by alphabetical order.
        var alphabeticallySortedAnimeList = animeList.OrderBy(ap => ap.AnimeName).ToList();

        var vm = new PlatformIndexViewModel()
        {
            Platform = platform,
            AnimeList = alphabeticallySortedAnimeList
        };

        return View("PlatformIndex", vm);
    }

    public IActionResult AnimePage(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ap = _context.AnimePages!.Find(id);

        if (ap != null)
        {
            return View("AnimePageDetails", ap);
        }
        else
        {
            return NotFound();
        }

    }

    [Authorize]
    public IActionResult CreateEpisode(int animePageId)
    {
        ViewData["reffer"] = Request.Headers["Referer"].ToString();
        return View("AnimeEpisodeCreate", new AnimeEpisode()
        {
            AnimePageId = animePageId
        });
    }

    [Authorize]
    public async Task<IActionResult> ModifyEpisode(int animeEpisodeId)
    {
        ViewData["reffer"] = Request.Headers["Referer"].ToString();
        var episode = await _unitOfWork.AnimeEpisodeRepository.GetByIdAsync(animeEpisodeId);

        if (episode == null) 
            return NotFound();

        return View("AnimeEpisodeEdit", episode);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ModifyEpisode(int id, [Bind("Id,EpisodeNumber,EpisodeName,EpisodeSummary,EpisodeContainsFiller,AnimePageId")] AnimeEpisode animeEpisode)
    {
        if (id != animeEpisode.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.AnimeEpisodeRepository.Update(animeEpisode);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
            return RedirectToAction("AnimePageDetails", "Anime", new { id = animeEpisode.AnimePageId });
        }
        return View("AnimeEpisodeEdit", animeEpisode);
    }

    /// <summary>
    /// GET: Anime/DeleteEpisode/5
    /// </summary>
    /// <param name="id">ID of the episode you want deleted. </param>
    /// <returns></returns>
    public async Task<IActionResult> DeleteEpisode(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var episode = await _unitOfWork.AnimeEpisodeRepository.GetByIdAsync(id);

        if (episode == null)
        {
            return NotFound();
        }

        ViewData["reffer"] = Request.Headers["Referer"].ToString();
        return View("AnimeEpisodeDelete", episode);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEpisodeConfirmed(int id)
    {
        var episode = await _unitOfWork.AnimeEpisodeRepository.GetByIdAsync(id);

        if (episode == null)
        {
            return NotFound();
        }

        var parentAnimeId = episode.AnimePageId;
        _unitOfWork.AnimeEpisodeRepository.Remove(episode);
        await _unitOfWork.CompleteAsync();

        return RedirectToAction("AnimePageDetails", new { id = parentAnimeId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,EpisodeNumber,EpisodeName,EpisodeSummary,EpisodeContainsFiller,AnimePageId")] AnimeEpisode animeEpisode)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.AnimeEpisodeRepository.AddAsync(animeEpisode);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction("AnimePage", "Anime", new { id = animeEpisode.AnimePageId });
        }

        return View("AnimeEpisodeCreate", animeEpisode);
    }

    [Authorize]
    public IActionResult CreateAnimePage()
    {
        ViewData["reffer"] = Request.Headers["Referer"].ToString();
        return View("AnimePageCreate");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAnimePage([Bind("AnimeName,AnimeSummary")] AnimePage animePage)
    {
        animePage.DownVotes = 0;
        animePage.UpVotes = 0;

        if (ModelState.IsValid)
        {
            await _unitOfWork.AnimePageRepository.AddAsync(animePage);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction("AnimePageDetails", "Anime", new { id = animePage.Id });
        }
        return View("AnimePageCreate", animePage);
    }

    public async Task<IActionResult> AnimePageDetails(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var animePage = await _unitOfWork.AnimePageRepository.GetByIdAsync(id);

        if (animePage == null)
        {
            return NotFound();
        }

        animePage.AnimeSummary = _htmlCleaner.Clean(animePage.AnimeSummary);
        return View("AnimePageDetails", animePage);
    }
}
