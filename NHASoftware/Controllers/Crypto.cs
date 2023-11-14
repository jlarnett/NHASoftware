using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace NHA.Website.Software.Controllers
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
