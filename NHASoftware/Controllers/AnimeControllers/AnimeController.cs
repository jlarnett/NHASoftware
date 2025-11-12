using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.IdentityModel.Tokens;
using NHA.Helpers.AlphabetSimplify;
using NHA.Helpers.HtmlStringCleaner;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Views.Anime.Vms;
namespace NHA.Website.Software.Controllers.AnimeControllers;

[FeatureGate("AnimeEnabled")]
public class AnimeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHtmlStringCleaner _htmlCleaner;

    public AnimeController(ApplicationDbContext context, IUnitOfWork unitOfWork, IHtmlStringCleaner htmlCleaner)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _htmlCleaner = htmlCleaner;
    }

    public IActionResult Index()
    {
        return View();
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
        char letter = AlphabetDecipher.ConvertNumberToAlphabetLetter(id);
        var completeAnimeList = await _unitOfWork.AnimePageRepository.GetAllAsync();

        List<AnimePage> animeList = new List<AnimePage>();

        //Getting all anime that starts with specific alphabet letter
        foreach (var anime in completeAnimeList)
        {
            if(anime.AnimeName == null) continue;
            
            if (anime.AnimeName.StartsWith(letter))
            {
                animeList.Add(anime);
            }
        }

        //Sorting the list by alphabetical order.
        var AlphabeticallySortedAnimesForLetter = animeList.OrderBy(ap => ap.AnimeName).ToList();

        var vm = new LetterIndexViewModel()
        {
            AlphabetLetter = letter,
            AnimeList = AlphabeticallySortedAnimesForLetter
        };

        return View("LetterIndex", vm);
    }

    public async Task<IActionResult> GenreIndex(string genre)
    {
        var completeAnimeList = await _unitOfWork.AnimePageRepository.GetAllAsync();

        List<AnimePage> animeList = [];

        //Getting all anime that starts with specific alphabet letter
        foreach (var anime in completeAnimeList)
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
        var completeAnimeList = await _unitOfWork.AnimePageRepository.GetAllAsync();

        List<AnimePage> animeList = [];

        //Getting all anime that starts with specific alphabet letter
        foreach (var anime in completeAnimeList)
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
