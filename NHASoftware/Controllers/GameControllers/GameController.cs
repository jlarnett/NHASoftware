using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.FeatureManagement.Mvc;
using NHA.Helpers.AlphabetSimplify;
using NHA.Website.Software.Caching;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Game;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Views.Game.GameVms;
using NHA.Website.Software.Views.Game.Vms;

namespace NHA.Website.Software.Controllers.GameControllers;

[FeatureGate("GameWikiEnabled")]
public class GameController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _memoryCache;

    public GameController(ApplicationDbContext context, IUnitOfWork unitOfWork, IMemoryCache memoryCache)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _memoryCache = memoryCache;
    }

    public async Task<IActionResult> Index()
    {
        //Try to get game pages from local cache
        if (!_memoryCache.TryGetValue(CachingKeys.Games, out IEnumerable<GamePage>? gamePages))
        {
            gamePages = await _unitOfWork.GamePageRepository.GetAllAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(12));

            _memoryCache.Set(CachingKeys.Games, gamePages, cacheEntryOptions);
        }

        return View(new IndexViewModel()
        {
            GameList = gamePages ?? []
        });
    }

    public async Task<IActionResult> LetterDetail(int id)
    {
        var letter = AlphabetDecipher.ConvertNumberToAlphabetLetter(id);

        //Try to get game pages from local cache
        if (!_memoryCache.TryGetValue(CachingKeys.Games, out IEnumerable<GamePage>? gamePages))
        {
            gamePages = await _unitOfWork.GamePageRepository.GetAllAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(12));

            _memoryCache.Set(CachingKeys.Games, gamePages, cacheEntryOptions);
        }

        var gameList = new List<GamePage>();

        //Getting all anime that starts with specific alphabet letter
        foreach (var game in gamePages ?? [])
        {
            if (game.Name.StartsWith(letter))
            {
                gameList.Add(game);
            }
        }

        //Sorting the list by alphabetical order.
        var alphabeticallySortedGamesForLetter = gameList.OrderBy(ap => ap.Name).ToList();

        var vm = new LetterGameIndexViewModel()
        {
            AlphabetLetter = letter,
            GameList = alphabeticallySortedGamesForLetter
        };

        return View("LetterIndex", vm);
    }
    public async Task<IActionResult> GenreIndex(string genre)
    {
        //Try to get game pages from local cache
        if (!_memoryCache.TryGetValue(CachingKeys.Games, out IEnumerable<GamePage>? gamePages))
        {
            gamePages = await _unitOfWork.GamePageRepository.GetAllAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(12));

            _memoryCache.Set(CachingKeys.Games, gamePages, cacheEntryOptions);
        }

        List<GamePage> gameList = [];

        //Getting all anime that starts with specific alphabet letter
        foreach (var game in gamePages ?? [])
        {
            if (game.Genres == null) continue;

            if (game.Genres.Contains(genre))
            {
                gameList.Add(game);
            }
        }

        //Sorting the list by alphabetical order.
        var alphabeticallySortedGameList = gameList.OrderBy(ap => ap.Name).ToList();

        var vm = new GenreIndexViewModel()
        {
            Genre = genre,
            GameList = alphabeticallySortedGameList
        };

        return View("GenreIndex", vm);
    }

    public async Task<IActionResult> PlatformIndex(string platform)
    {
        //Try to get game pages from local cache
        if (!_memoryCache.TryGetValue(CachingKeys.Games, out IEnumerable<GamePage>? gamePages))
        {
            gamePages = await _unitOfWork.GamePageRepository.GetAllAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(12));

            _memoryCache.Set(CachingKeys.Games, gamePages, cacheEntryOptions);
        }

        List<GamePage> gameList = [];

        //Getting all anime that starts with specific alphabet letter
        foreach (var game in gamePages ?? [])
        {
            if (game.Platforms == null) continue;

            if (game.Platforms.Contains(platform))
            {
                gameList.Add(game);
            }
        }

        //Sorting the list by alphabetical order.
        var alphabeticallySortedGameList = gameList.OrderBy(ap => ap.Name).ToList();

        var vm = new PlatformIndexViewModel()
        {
            Platform = platform,
            GameList = alphabeticallySortedGameList
        };

        return View("PlatformIndex", vm);
    }

    public IActionResult GamePage(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ap = _context.GamePages!.Find(id);

        if (ap != null)
        {
            return View("GamePageDetails", ap);
        }
        
        return NotFound();
    }

}
