using Microsoft.AspNetCore.Mvc;
using NHASoftware.Models;
using System.Diagnostics;
using NHASoftware.Data;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            int authorCount = _context.Authors.Count();
            int bookCount = _context.Books.Count();

            return View(new IndexPageViewModel(authorCount, bookCount));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}