#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.Models.ForumModels;

namespace NHASoftware.Controllers
{
    public class ForumCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ForumComments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumComments.Include(f => f.ForumPost);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ForumComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumComment = await _context.ForumComments
                .Include(f => f.ForumPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (forumComment == null)
            {
                return NotFound();
            }

            return View(forumComment);
        }

        // GET: ForumComments/Create
        public IActionResult Create()
        {
            ViewData["ForumPostId"] = new SelectList(_context.ForumPost, "Id", "Id");
            return View();
        }

        // POST: ForumComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CommentText,CreationDate,UserId,ForumPostId")] ForumComment forumComment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(forumComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ForumPostId"] = new SelectList(_context.ForumPost, "Id", "Id", forumComment.ForumPostId);
            return View(forumComment);
        }

        // GET: ForumComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumComment = await _context.ForumComments.FindAsync(id);
            if (forumComment == null)
            {
                return NotFound();
            }
            ViewData["ForumPostId"] = new SelectList(_context.ForumPost, "Id", "Id", forumComment.ForumPostId);
            return View(forumComment);
        }

        // POST: ForumComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CommentText,CreationDate,UserId,ForumPostId")] ForumComment forumComment)
        {
            if (id != forumComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumCommentExists(forumComment.Id))
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
            ViewData["ForumPostId"] = new SelectList(_context.ForumPost, "Id", "Id", forumComment.ForumPostId);
            return View(forumComment);
        }

        // GET: ForumComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumComment = await _context.ForumComments
                .Include(f => f.ForumPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (forumComment == null)
            {
                return NotFound();
            }

            return View(forumComment);
        }

        // POST: ForumComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumComment = await _context.ForumComments.FindAsync(id);
            _context.ForumComments.Remove(forumComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumCommentExists(int id)
        {
            return _context.ForumComments.Any(e => e.Id == id);
        }
    }
}
