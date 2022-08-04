using Microsoft.AspNetCore.Mvc;
using NHASoftware.HelperClasses;
using NHASoftware.ViewModels.AnimeVMs;

namespace NHASoftware.Controllers.AnimeControllers
{
    public class AnimeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LetterDetail(int id)
        {
            char letter = AlphabetDecipher.ConvertNumberToAlphabetLetter(id);
            var vm = new LetterIndexViewModel()
            {
                AlphabetLetter = letter
            };
            return View("LetterIndex", vm);
        }
    }
}
 