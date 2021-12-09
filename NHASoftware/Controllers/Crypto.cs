using Microsoft.AspNetCore.Mvc;

namespace NHASoftware.Controllers
{
    public class Crypto : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
