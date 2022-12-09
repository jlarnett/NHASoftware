using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using NHASoftware.Controllers.WebAPIs;
using NHASoftware.DBContext;
using NHASoftware.Entities.Identity;
using NHASoftware.HelperClasses;
using NHASoftware.Services;
using NHASoftware.Services.CookieMonster;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        private ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICookieMonster _cookieMonster;
        private TaskHandler taskHandler;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            IEmailSender emailService,
            UserManager<ApplicationUser> userManager, 
            ICookieMonster cookieMonster)
        {
            /*************************************************************************************
             *  Dependency injection services
             *************************************************************************************/

            _logger = logger;
            _context = context;
            _userManager = userManager;
            _cookieMonster = cookieMonster;
            this.taskHandler = new TaskHandler(context, userManager, emailService);
        }

        public IActionResult Index()
        {
            CreatePrimaryHangfireJobs();
            CreateDailyInactiveCheckJob();

            int subCount = _context.Subscriptions.Count();
            int taskCount = _context.Tasks.Count();

            AssignSessionGuidCookie();

            return View(new IndexPageViewModel(subCount, taskCount));
        }

        private void AssignSessionGuidCookie()
        {
            _logger.LogInformation("Trying to assign cookies at {DT}", DateTime.UtcNow.ToLongTimeString());
            var sessionId = _cookieMonster.TryRetrieveCookie(CookieKeys.Session);

            if (sessionId == null)
            {
                var sessionGuid = Guid.NewGuid();
                _cookieMonster.CreateCookie(CookieKeys.Session, sessionGuid.ToString());
            }
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
            RecurringJob.AddOrUpdate("Morning Task Check", () => taskHandler.CreateNewTaskJobs(), "0 6 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("Noon Task Check", () => taskHandler.CreateNewTaskJobs(), "0 12 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("Evening Day Task Check", () => taskHandler.CreateNewTaskJobs(), "0 18 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("Night Day Task Check", () => taskHandler.CreateNewTaskJobs(), "0 23 * * *", TimeZoneInfo.Local);
        }
        private void CreateDailyInactiveCheckJob()
        {
            RecurringJob.AddOrUpdate("Outdated Account TaskHandler Job Clear", () => taskHandler.ClearDatedJobs(), "0 6 * * *", TimeZoneInfo.Local);
        }
    }
}