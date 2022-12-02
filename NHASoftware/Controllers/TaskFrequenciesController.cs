#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHASoftware.DBContext;
using NHASoftware.Entities;

namespace NHASoftware.Controllers
{
    public class TaskFrequenciesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskFrequenciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaskFrequencies
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Frequencies.ToListAsync());
        }

        // GET: TaskFrequencies/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskFrequency = await _context.Frequencies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskFrequency == null)
            {
                return NotFound();
            }

            return View(taskFrequency);
        }

        // GET: TaskFrequencies/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaskFrequencies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,FrequencyName,FrequencyValue")] TaskFrequency taskFrequency)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskFrequency);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskFrequency);
        }

        // GET: TaskFrequencies/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskFrequency = await _context.Frequencies.FindAsync(id);
            if (taskFrequency == null)
            {
                return NotFound();
            }
            return View(taskFrequency);
        }

        // POST: TaskFrequencies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FrequencyName,FrequencyValue")] TaskFrequency taskFrequency)
        {
            if (id != taskFrequency.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskFrequency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskFrequencyExists(taskFrequency.Id))
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
            return View(taskFrequency);
        }

        // GET: TaskFrequencies/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskFrequency = await _context.Frequencies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskFrequency == null)
            {
                return NotFound();
            }

            return View(taskFrequency);
        }

        // POST: TaskFrequencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskFrequency = await _context.Frequencies.FindAsync(id);
            _context.Frequencies.Remove(taskFrequency);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskFrequencyExists(int id)
        {
            return _context.Frequencies.Any(e => e.Id == id);
        }
    }
}
