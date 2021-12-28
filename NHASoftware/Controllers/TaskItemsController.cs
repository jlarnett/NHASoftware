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
using NHASoftware.Services;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly  UserManager<IdentityUser> _userManager;
        private FrequencyHandler frequencyHandler;

        public TaskItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailService emailService)
        {
            /*******************************************************************************************
             *  I believe dependency injection is where controllers receive service items.
             *  Instantiated in the controller constructor so it can be managed simply.
             ********************************************************************************************/

            _context = context;
            _userManager = userManager;
            frequencyHandler = new FrequencyHandler(_context, emailService);
        }

        public async Task<IActionResult> Index()
        {
            /******************************************************************************************************
             *  GET: TaskItems
             *  Returns a list of Task for the specified user. 
             *******************************************************************************************************/

            var applicationDbContext = _context.Tasks.Include(t => t.Frequency).Include(t => t.User).Where(u => u.UserId == _userManager.GetUserId(HttpContext.User));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TaskItems/Details/5
        [Authorize]
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

        [Authorize]
        public IActionResult Create()
        {
            /******************************************************************************************************
            *  GET: TaskItems/Create
            *  Creates view model and assigns the userId and task start date of today.
            *******************************************************************************************************/

            ViewData["FrequencyId"] = new SelectList(_context.Frequencies, "Id", "FrequencyName");

            var vm = new TaskFormViewModel()
            {
                TaskStartDate = DateTime.Today,
                UserId = _userManager.GetUserId(HttpContext.User)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("TaskId,TaskDescription,TaskStartDate,TaskExecutionTime,FrequencyId, UserId")] TaskFormViewModel taskVM)
        {
            /*******************************************************************************************
             *      POST: TaskItems/Create
             *      Create the task item from view model.
             *******************************************************************************************/

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

        public async Task<IActionResult> Edit(int? id)
        {
            /****************************************************************************************
            *      POST: TaskItems/Edit/5
            *      NOT IMPLEMENTED
            ****************************************************************************************/

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,TaskDescription,TaskIsFinished,TaskStartDate,TaskExecutionTime,FrequencyId,UserId")] TaskItem taskItem)
        {
            /****************************************************************************************
            *      POST: TaskItems/Edit/5
            *      NOT IMPLEMENTED
            ****************************************************************************************/

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


        public async Task<IActionResult> Delete(int? id)
        {
            /****************************************************************************************
            *      GET: TaskItems/Delete/5
            *      Delete controller action takes task id & checks whether it exist. If found
            *      Returns the 'Delete' confirmation view. 
            ****************************************************************************************/

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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            /****************************************************************************************
             *      POST: TaskItems/Delete/5
             *      Deletes the task & removes the recurring job from Hangfire. 
             ****************************************************************************************/

            var taskItem = await _context.Tasks.FindAsync(id);

            _context.Tasks.Remove(taskItem);
            RecurringJob.RemoveIfExists("TaskId: " + taskItem.TaskId);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool TaskItemExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}


