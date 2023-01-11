using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using NHASoftware.Controllers.WebAPIs;
using NHASoftware.DBContext;
using NHASoftware.Entities.Identity;
using NHASoftware.HelperClasses;
using NHASoftware.Services.CookieMonster;
using NHASoftware.Services.RepositoryPatternFoundationals;
using NHASoftware.ViewModels;
using NHASoftware.Views.ViewModels.SocialVMs;

namespace NHASoftware.Controllers
{
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICookieMonster _cookieMonster;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private TaskHandler taskHandler;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            IEmailSender emailService,
            UserManager<ApplicationUser> userManager, 
            ICookieMonster cookieMonster,
            IMapper mapper, IUnitOfWork unitOfWork)
        {
            /*************************************************************************************
             *  Dependency injection services
             *************************************************************************************/

            _logger = logger;
            _userManager = userManager;
            _cookieMonster = cookieMonster;
            this._mapper = mapper;
            this._unitOfWork = unitOfWork;
            this.taskHandler = new TaskHandler(context, userManager, emailService);
        }

        public IActionResult Index()
        {
            CreatePrimaryHangfireJobs();
            CreateDailyInactiveCheckJob();
            AssignSessionGuidCookie();

            //var posts = await new PostsController(_mapper, _unitOfWork).GetPosts();
            return View();
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