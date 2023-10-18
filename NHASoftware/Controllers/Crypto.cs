using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace NHASoftware.Controllers
{
    [FeatureGate("CryptoEnabled")]
    public class Crypto : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
