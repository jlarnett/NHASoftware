using Microsoft.AspNetCore.Mvc;
using NHASoftware.Models;
using System.Diagnostics;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using NHASoftware.Data;
using NHASoftware.HelperClasses;
using NHASoftware.Services;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        private ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private FrequencyHandler frequencyHandler;
        private TaskHandler taskHandler;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IEmailSender emailService, UserManager<ApplicationUser> userManager)
        {
            /*************************************************************************************
             *  Dependency injection services
             *************************************************************************************/

            _logger = logger;
            _context = context;
            _userManager = userManager;

            frequencyHandler = new FrequencyHandler(context, emailService);
            this.taskHandler = new TaskHandler(context, userManager);
        }

        public IActionResult Index()
        {

            CreatePrimaryHangfireJobs();
            CreateDailyInactiveCheckJob();

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
            RecurringJob.AddOrUpdate("Evening Task Check", () => frequencyHandler.GetRelevantTask(), "0 20 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("Morning Task Check",() => frequencyHandler.GetRelevantTask(), "0 6 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("Noon Task Check",() => frequencyHandler.GetRelevantTask(), "0 12 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("Late Day Task Check", () => frequencyHandler.GetRelevantTask(), "0 18 * * *", TimeZoneInfo.Local);
        }
        private void CreateDailyInactiveCheckJob()
        {
            RecurringJob.AddOrUpdate("TaskHandler", () => taskHandler.ClearDatedJobs(), "0 6 * * *", TimeZoneInfo.Local);
        }
    }
}