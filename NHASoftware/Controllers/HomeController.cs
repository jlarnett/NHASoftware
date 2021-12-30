using Microsoft.AspNetCore.Mvc;
using NHASoftware.Models;
using System.Diagnostics;
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
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

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IEmailSender emailService)
        {
            /*************************************************************************************
             *  Dependency injection services
             *************************************************************************************/
            _logger = logger;
            _context = context;
            frequencyHandler = new FrequencyHandler(context, emailService);
        }

        public IActionResult Index()
        {

            CreatePrimaryHangfireJobs();

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

        private void CreatePrimaryHangfireJobs()
        {
            RecurringJob.AddOrUpdate(() => frequencyHandler.GetRelevantTask(), "0 20 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => frequencyHandler.GetRelevantTask(), "0 6 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => frequencyHandler.GetRelevantTask(), "0 12 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => frequencyHandler.GetRelevantTask(), "0 18 * * *", TimeZoneInfo.Local);
        }
    }
}