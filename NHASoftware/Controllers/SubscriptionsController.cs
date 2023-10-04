using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using NHASoftware.DBContext;
using NHASoftware.Entities;
using NHASoftware.Entities.Identity;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly  UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SubscriptionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Subscriptions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Subscriptions.Where(c => c.UserId == _userManager.GetUserId(HttpContext.User)).ToListAsync());
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(m => m.SubscriptionId == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }


        [Authorize]
        public IActionResult Create()
        {
            // GET: Subscriptions/Create
            /***************************************************************************************************
            *  Returns a Create View. SelectList is populated with all task related to userId.
            ***************************************************************************************************/

            var  list = new SelectList(_context.Tasks.Where(c => c.UserId == _userManager.GetUserId(HttpContext.User)), "TaskId", "TaskDescription");
            var list2 = list.Prepend(new SelectListItem()
            {
                Value = "-1",
                Text = ""

            });

            ViewData["TaskId"] = list2;

            var vm = new SubscriptionFormViewModel()
            {
                UserId =  _userManager.GetUserId(HttpContext.User),
                User = _context.Users.Find(_userManager.GetUserId(HttpContext.User)) as ApplicationUser,
                SubscriptionDate = DateTime.Today
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("SubscriptionId,SubscriptionName,SubscriptionDate,SubscriptionCost,User,UserId,TaskItemId")] SubscriptionFormViewModel vm)
        {
            // POST: Subscriptions/Create
            /***************************************************************************************************
            *  Creates new subscription. Takes SubscriptionFormViewModel.
            *  Each Subscription allows a linked task now. That can be tied to hangfire system. 
            ***************************************************************************************************/


            var subscription = new Subscription()
            {
                SubscriptionDay = vm.SubscriptionDay,
                SubscriptionName = vm.SubscriptionName,
                SubscriptionDate = vm.SubscriptionDate,
                SubscriptionCost = vm.SubscriptionCost,
                User = vm.User,
                UserId = vm.UserId
            };

            if (vm.TaskItemId != null && vm.TaskItemId != -1)
            {
                subscription.TaskItemId = vm.TaskItemId.Value;
            }

            if (ModelState.IsValid)
            {

                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var  list = new SelectList(_context.Tasks.Where(c => c.UserId == _userManager.GetUserId(HttpContext.User)), "TaskId", "TaskDescription");
            var list2 = list.Prepend(new SelectListItem()
            {
                Value = "-1",
                Text = ""

            });

            ViewData["TaskId"] = list2;

            return View("Index");
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            // GET: Subscriptions/Edit/5
            /***************************************************************************************************
            *  Edits Subscription. Creates list prepend to allow blank selection
            ***************************************************************************************************/

            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);

            if (subscription == null)
            {
                return NotFound();
            }

            if (_userManager.GetUserId(HttpContext.User) != subscription.UserId)
            {
                return RedirectToAction("Index", "Subscriptions");
            }

            var vm = new SubscriptionFormViewModel()
            {
                UserId =  subscription.UserId,
                TaskItemId = subscription.TaskItemId,
                SubscriptionCost = subscription.SubscriptionCost,
                SubscriptionDate = subscription.SubscriptionDate,
                SubscriptionName = subscription.SubscriptionName,
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionDay = subscription.SubscriptionDay
            };

            var  list = new SelectList(_context.Tasks.Where(c => c.UserId == _userManager.GetUserId(HttpContext.User)), "TaskId", "TaskDescription");
            var list2 = list.Prepend(new SelectListItem()
            {
                Value = "-1",
                Text = ""

            });

            ViewData["TaskId"] = list2;
            return View(vm);
        }

        // POST: Subscriptions/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubscriptionId,SubscriptionName,SubscriptionDate,SubscriptionCost,User,UserId,TaskItemId")] SubscriptionFormViewModel subscriptionVM)
        {
            if (id != subscriptionVM.SubscriptionId)
            {
                return NotFound();
            }

            if (_userManager.GetUserId(HttpContext.User) != subscriptionVM.UserId)
            {
                return NotFound();
            }

            var subscription = new Subscription()
            {
                UserId =  subscriptionVM.UserId,
                SubscriptionCost = subscriptionVM.SubscriptionCost,
                SubscriptionDate = subscriptionVM.SubscriptionDate,
                SubscriptionName = subscriptionVM.SubscriptionName,
                SubscriptionId = subscriptionVM.SubscriptionId,
                SubscriptionDay = subscriptionVM.SubscriptionDay
            };

            if (subscriptionVM.TaskItemId != null && subscriptionVM.TaskItemId != -1)
            {
                subscription.TaskItemId = subscriptionVM.TaskItemId.Value;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.SubscriptionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var  list = new SelectList(_context.Tasks.Where(c => c.UserId == _userManager.GetUserId(HttpContext.User)), "TaskId", "TaskDescription");
            var list2 = list.Prepend(new SelectListItem()
            {
                Value = "-1",
                Text = ""

            });

            ViewData["TaskId"] = list2;

            return View(subscriptionVM);
        }

        // GET: Subscriptions/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(m => m.SubscriptionId == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return BadRequest();
            }
            _context.Subscriptions.Remove(subscription);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(int id)
        {
            /***************************************************************************************************
            *  Checks if subscription with id exist in database.
            ***************************************************************************************************/

            return _context.Subscriptions.Any(e => e.SubscriptionId == id);
        }

        [Authorize]
        public async Task<IActionResult> GenerateSubscriptionReport()
        {
            /***************************************************************************************************
             *  Gets All User subscriptions from database context
             *  Gets the Root path and finds correct folder for report.
             *  Uses streamWriter to write formatted strings to file. 
             ***************************************************************************************************/

            decimal totalPrice = 0;

            string UserId = _userManager.GetUserId(HttpContext.User);
            List<Subscription> subscriptions = _context.Subscriptions.Where(c => c.UserId == UserId).ToList();

            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "Reports");

            using StreamWriter file = new(path + UserId + ".txt");

            foreach (var sub in subscriptions)
            {
                string subscriptionString = String.Format("|{0,-30}|{1,-5}|{2,-10}", sub.SubscriptionName, sub.SubscriptionDate.Day.ToString(), sub.SubscriptionCost.ToString());
                await file.WriteLineAsync(subscriptionString);
                totalPrice += sub.SubscriptionCost;
            }

            string total = "Total Monthly Cost";
            string totalPriceString = String.Format("|{0,-36}|${1,-10}", total, totalPrice.ToString());

            await file.WriteLineAsync(totalPriceString);
            return RedirectToAction("Index");
        }
    }
}
