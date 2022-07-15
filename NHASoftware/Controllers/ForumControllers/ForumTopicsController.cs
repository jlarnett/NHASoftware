#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.Models.ForumModels;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class ForumTopicsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumTopicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ForumTopics
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumTopics.Include(f => f.ForumSection);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ForumTopics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topicPosts = _context.ForumPost.Where(c => c.ForumTopicId == id).ToList();

            var forumTopic = await _context.ForumTopics
                .Include(f => f.ForumSection)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (forumTopic == null)
            {
                return NotFound();
            }

            foreach (var post in topicPosts)
            {
                //Replaces the forumText line breaks with correct html element
                post.ForumText = Regex.Replace(post.ForumText, @"\r\n?|\n", "<br>");
            }

            var vm = new ForumTopicDetailsView()
            {
                topic = forumTopic,
                Posts = topicPosts
            };

            return View(vm);
        }

        // GET: ForumTopics/Create
        public IActionResult Create()
        {
            ViewData["ForumSectionId"] = new SelectList(_context.ForumSections, "Id", "Name");

            var topic = new ForumTopic()
            {
                PostCount = 0,
                ThreadCount = 0
            };
            
            ViewData["reffer"] = Request.Headers["Referer"].ToString();
            return View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ForumSectionId,PostCount, ThreadCount")] ForumTopic forumTopic)
        {
            /****************************************************************************************************
             *  POST: ForumTopics/Create
             ***************************************************************************************************/

            if (ModelState.IsValid)
            {
                _context.Add(forumTopic);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Forum");
            }
            ViewData["ForumSectionId"] = new SelectList(_context.ForumSections, "Id", "Name", forumTopic.ForumSectionId);
            return View(forumTopic);
        }

        // GET: ForumTopics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics.FindAsync(id);
            if (forumTopic == null)
            {
                return NotFound();
            }
            ViewData["ForumSectionId"] = new SelectList(_context.ForumSections, "Id", "Id", forumTopic.ForumSectionId);
            return View(forumTopic);
        }

        // POST: ForumTopics/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ForumSectionId")] ForumTopic forumTopic)
        {
            if (id != forumTopic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumTopic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumTopicExists(forumTopic.Id))
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
            ViewData["ForumSectionId"] = new SelectList(_context.ForumSections, "Id", "Id", forumTopic.ForumSectionId);
            return View(forumTopic);
        }

        // GET: ForumTopics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(f => f.ForumSection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (forumTopic == null)
            {
                return NotFound();
            }

            return View(forumTopic);
        }

        // POST: ForumTopics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumTopic = await _context.ForumTopics.FindAsync(id);
            _context.ForumTopics.Remove(forumTopic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumTopicExists(int id)
        {
            return _context.ForumTopics.Any(e => e.Id == id);
        }
    }
}
