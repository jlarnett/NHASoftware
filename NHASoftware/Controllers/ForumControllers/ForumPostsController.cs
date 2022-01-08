#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.Models;
using NHASoftware.Models.ForumModels;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class ForumPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForumPostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ForumPosts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumPost.Include(f => f.ForumTopic);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ForumPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumPost = await _context.ForumPost
                .Include(f => f.ForumTopic)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (forumPost == null)
            {
                return NotFound();
            }

            var detailVm = new ForumPostDetailModel()
            {
                ForumPost = forumPost,
                ForumComments = _context.ForumComments.Where(c => c.ForumPostId == id).Include(p => p.User).ToList()
            };

            return View(detailVm);
        }

        // GET: ForumPosts/Create
        [Authorize]
        public IActionResult Create(int id)
        {
            var forumPost = new ForumPost();
            forumPost.UserId = _userManager.GetUserId(HttpContext.User);
            forumPost.ForumTopicId = id;
            forumPost.CreationDate = DateTime.Now;
            forumPost.CommentCount = 0;

            return View(forumPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ForumText,CreationDate, UserId, ForumTopicId, CommentCount")] ForumPost forumPost)
        {
            /*******************************************************************************************************
             *      POST: ForumPosts/Create
             *      Adds forumPost to database if the model is valid.
             *      Increments the Post Count & Thread Count & latestPost.
             *******************************************************************************************************/

            if (ModelState.IsValid)
            {
                var topic = await _context.ForumTopics.FirstAsync(c => c.Id == forumPost.ForumTopicId);

                topic.PostCount += 1;
                topic.ThreadCount += 1;
                topic.LastestPost = DateTime.Now;

                _context.Add(forumPost);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "ForumTopics", new {id = forumPost.ForumTopicId});
            }
            return View(forumPost);
        }

        // GET: ForumPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumPost = await _context.ForumPost.FindAsync(id);
            if (forumPost == null)
            {
                return NotFound();
            }
            ViewData["ForumTopicId"] = new SelectList(_context.ForumTopics, "Id", "Id", forumPost.ForumTopicId);
            return View(forumPost);
        }

        // POST: ForumPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ForumText,CreationDate,UserId,ForumTopicId")] ForumPost forumPost)
        {
            if (id != forumPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumPostExists(forumPost.Id))
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
            ViewData["ForumTopicId"] = new SelectList(_context.ForumTopics, "Id", "Id", forumPost.ForumTopicId);
            return View(forumPost);
        }

        // GET: ForumPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumPost = await _context.ForumPost
                .Include(f => f.ForumTopic)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (forumPost == null)
            {
                return NotFound();
            }

            return View(forumPost);
        }

        // POST: ForumPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumPost = await _context.ForumPost.FindAsync(id);
            _context.ForumPost.Remove(forumPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumPostExists(int id)
        {
            return _context.ForumPost.Any(e => e.Id == id);
        }
    }
}
