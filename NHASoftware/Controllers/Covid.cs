using Microsoft.AspNetCore.Mvc;

namespace NHASoftware.Controllers
{
    public class Covid : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
