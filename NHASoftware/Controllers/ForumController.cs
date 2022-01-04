using Microsoft.AspNetCore.Mvc;

namespace NHASoftware.Controllers
{
    public class ForumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
