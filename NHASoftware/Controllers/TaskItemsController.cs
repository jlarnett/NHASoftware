#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.Models;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly  UserManager<IdentityUser> _userManager;
        private FrequencyHandler frequencyHandler;


        public TaskItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            frequencyHandler = new FrequencyHandler(_context);
        }

        // GET: TaskItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tasks.Include(t => t.Frequency).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TaskItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks
                .Include(t => t.Frequency)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // GET: TaskItems/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["FrequencyId"] = new SelectList(_context.Frequencies, "Id", "FrequencyName");

            var vm = new TaskFormViewModel()
            {
                TaskStartDate = DateTime.Today,
                UserId = _userManager.GetUserId(HttpContext.User)
            };
            return View(vm);
        }

        // POST: TaskItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,TaskDescription,TaskStartDate,TaskExecutionTime,FrequencyId, UserId")] TaskFormViewModel taskVM)
        {

            var taskItem = new TaskItem()
            {
                TaskExecutionTime = taskVM.TaskExecutionTime.Value,
                TaskDescription = taskVM.TaskDescription,
                TaskStartDate = taskVM.TaskStartDate,
                FrequencyId = taskVM.FrequencyId,
                UserId = taskVM.UserId,
                JobCrated = false,
                NextTaskDate = frequencyHandler.GenerateNextDate(taskVM.TaskStartDate, _context.Frequencies.Find(taskVM.FrequencyId))
            };

            if (ModelState.IsValid)
            {
                //Cron string setup.
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FrequencyId"] = new SelectList(_context.Frequencies, "Id", "Id", taskItem.FrequencyId);
            return View("Index");
        }

        // GET: TaskItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }
            ViewData["FrequencyId"] = new SelectList(_context.Frequencies, "Id", "Id", taskItem.FrequencyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", taskItem.UserId);
            return View(taskItem);
        }

        // POST: TaskItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,TaskDescription,TaskIsFinished,TaskStartDate,TaskExecutionTime,FrequencyId,UserId")] TaskItem taskItem)
        {
            if (id != taskItem.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskItemExists(taskItem.TaskId))
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
            ViewData["FrequencyId"] = new SelectList(_context.Frequencies, "Id", "Id", taskItem.FrequencyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", taskItem.UserId);
            return View(taskItem);
        }

        // GET: TaskItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks
                .Include(t => t.Frequency)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // POST: TaskItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(taskItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskItemExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }


    }
}
