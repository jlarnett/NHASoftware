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

        /// <summary>
        /// GET: ForumTopics
        /// </summary>
        /// <returns>Returns the forumTopic index page.</returns>
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumTopics.Include(f => f.ForumSection);
            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary>
        /// GET: ForumTopics/Details/5
        /// </summary>
        /// <param name="id">Forum Topic Id you want the details for</param>
        /// <returns>Returns the Forum Topics Details</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topicPosts = _context.ForumPosts.Where(c => c.ForumTopicId == id).ToList();

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

        /// <summary>
        /// GET: ForumTopics/Create
        /// </summary>
        /// <returns>Returns the Create forum topic view</returns>
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

        /// <summary>
        /// POST: ForumTopics/Create
        /// Creates new forumTopic in database.
        /// </summary>
        /// <param name="forumTopic">Object the values are binded to</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ForumSectionId,PostCount, ThreadCount")] ForumTopic forumTopic)
        {

            if (ModelState.IsValid)
            {
                _context.Add(forumTopic);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Forum");
            }
            ViewData["ForumSectionId"] = new SelectList(_context.ForumSections, "Id", "Name", forumTopic.ForumSectionId);
            return View(forumTopic);
        }

        /// <summary>
        /// GET: ForumTopics/Edit/5
        /// Returns the forum Topic Edit View
        /// </summary>
        /// <param name="id">Forum Topic Id you want to update</param>
        /// <returns></returns>
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

            ViewData["reffer"] = Request.Headers["Referer"].ToString();
            ViewData["ForumSectionId"] = new SelectList(_context.ForumSections, "Id", "Id", forumTopic.ForumSectionId);
            return View(forumTopic);
        }

        /// <summary>
        /// POST: ForumTopics/Edit/5
        /// Updates the supplied forumTopic
        /// </summary>
        /// <param name="id">ID of the forum topic you want updated</param>
        /// <param name="forumTopic">Object the topic details are binded to</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ForumSectionId,ThreadCount,PostCount,LatestPost")] ForumTopic forumTopic)
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
                return RedirectToAction("Index", "Forum");
            }
            ViewData["ForumSectionId"] = new SelectList(_context.ForumSections, "Id", "Id", forumTopic.ForumSectionId);
            return View(forumTopic);
        }

        /// <summary>
        /// GET: ForumTopics/Delete/5
        /// Checks if the forum topic id exist & returns the Delete Confirmed View. 
        /// </summary>
        /// <param name="id">ID of the topic you want deleted. </param>
        /// <returns></returns>
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

            ViewData["reffer"] = Request.Headers["Referer"].ToString();
            return View(forumTopic);
        }

        /// <summary>
        /// POST: ForumTopics/Delete/5
        /// Deletes the supplied forum topic. 
        /// </summary>
        /// <param name="id">topic id you want deleted from database</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumTopic = await _context.ForumTopics.FindAsync(id);
            _context.ForumTopics.Remove(forumTopic);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Forum");
        }

        /// <summary>
        /// Checks if the forum topic exist
        /// </summary>
        /// <param name="id">Forum Topic Id</param>
        /// <returns>returns true if the forum topic exists</returns>
        private bool ForumTopicExists(int id)
        {
            return _context.ForumTopics.Any(e => e.Id == id);
        }
    }
}
