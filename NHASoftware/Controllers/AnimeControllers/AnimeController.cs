using Microsoft.AspNetCore.Mvc;
using NHASoftware.Data;
using NHASoftware.HelperClasses;
using NHASoftware.Models.AnimeModels;
using NHASoftware.ViewModels.AnimeVMs;

namespace NHASoftware.Controllers.AnimeControllers
{
    public class AnimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnimeController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LetterDetail(int id)
        {
            char letter = AlphabetDecipher.ConvertNumberToAlphabetLetter(id);
            var completeAnimeList = _context.AnimePages.ToList();
            List<AnimePage> animeList = new List<AnimePage>();

            //Getting all anime that starts with specific alphabet letter
            foreach (var anime in completeAnimeList)
            {
                if(anime.AnimeName.StartsWith(letter))
                {
                    animeList.Add(anime);
                }
            }

            //Sorting the list by alphabetical order.
            var AlphabeticallySortedAnimesForLetter  = animeList.OrderBy(ap => ap.AnimeName).ToList();

            var vm = new LetterIndexViewModel()
            {
                AlphabetLetter = letter,
                AnimeList = AlphabeticallySortedAnimesForLetter
            };

            return View("LetterIndex", vm);
        }

        public IActionResult AnimePage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ap = _context.AnimePages.Find(id);

            if (ap != null)
            {
                return View("AnimePageDetails", ap);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
 