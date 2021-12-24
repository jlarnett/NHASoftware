using Microsoft.AspNetCore.Mvc;
using NHASoftware.Models;
using System.Diagnostics;
using Hangfire;
using NHASoftware.Data;
using NHASoftware.Services;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private FrequencyHandler frequencyHandler;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            frequencyHandler = new FrequencyHandler(context, emailService);
        }

        public IActionResult Index()
        {
            RecurringJob.AddOrUpdate(() => frequencyHandler.GetRelevantTask(), "0 20 * * *", TimeZoneInfo.Local);

            int subCount = _context.Subscriptions.Count();
            int taskCount = _context.Tasks.Count();

            return View(new IndexPageViewModel(subCount, taskCount));
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