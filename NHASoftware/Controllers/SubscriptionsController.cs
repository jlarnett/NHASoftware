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
using NHASoftware.Data;
using NHASoftware.Models;

namespace NHASoftware.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly  UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SubscriptionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
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

        // GET: Subscriptions/Create
        [Authorize]
        public IActionResult Create()
        {
            Subscription model = new Subscription()
            {
                UserId =  _userManager.GetUserId(HttpContext.User),
                User = _context.Users.Find(_userManager.GetUserId(HttpContext.User))
            };

            return View(model);
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("SubscriptionId,SubscriptionName,SubscriptionDate,SubscriptionCost,User,UserId")] Subscription subscription)
        {

            if (ModelState.IsValid)
            {

                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);

            if (subscription == null)
            {
                return NotFound();
            }
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubscriptionId,SubscriptionName,SubscriptionDate,SubscriptionCost")] Subscription subscription)
        {
            if (id != subscription.SubscriptionId)
            {
                return NotFound();
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
            return View(subscription);
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
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(int id)
        {
            return _context.Subscriptions.Any(e => e.SubscriptionId == id);
        }

        [Authorize]
        public async Task<IActionResult> GenerateSubscriptionReport()
        {
            int counterCatch = 0;
            decimal totalPrice = 0;

            string UserId = _userManager.GetUserId(HttpContext.User);
            List<Subscription> subscriptions = _context.Subscriptions.Where(c => c.UserId == UserId).ToList();

            string[] lines = new string[subscriptions.Count + 1];

            for (int i = 0; i < subscriptions.Count; i++)
            {
                lines[i] = subscriptions[i].SubscriptionName + "        " + subscriptions[i].SubscriptionCost.ToString();
                totalPrice += subscriptions[i].SubscriptionCost;
                counterCatch = i;
            }

            lines[counterCatch + 1] = "Total Monthly Cost:    $" + totalPrice.ToString();

            string webRootPath = _webHostEnvironment.WebRootPath;

            string path = "";
            path = Path.Combine(webRootPath, "Reports");

            using StreamWriter file = new(path + UserId + ".txt");

            foreach (var line in lines)
            {
                await file.WriteLineAsync(line);
            }

            return RedirectToAction("Index");
        }
    }
}
