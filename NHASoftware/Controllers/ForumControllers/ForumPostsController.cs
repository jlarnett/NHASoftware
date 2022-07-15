#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
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
        //DI Services
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The forumId you want to view the details page of.</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            /*******************************************************************************************************
            *      GET: ForumPosts/Details
            *      Returns ForumPost Details View. Uses detailVm
            *******************************************************************************************************/

            if (id == null)
            {
                return NotFound();
            }

            var forumPost = await _context.ForumPost
                .Include(f => f.ForumTopic)
                .Include(f => f.User)
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

           
            detailVm.ForumPost.ForumText.Replace("\r\n", "<br/>");
            return View(detailVm);
        }

        // GET: ForumPosts/Create
        [Authorize]
        public IActionResult Create(int id)
        {
            /*******************************************************************************************************
            *      GET: ForumPosts/Create
            *      Returns forumPost Create View. Populates some server side information.
            *******************************************************************************************************/

            var forumPost = new ForumPost()
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ForumTopicId = id,
                CreationDate = DateTime.Now,
                CommentCount = 0
            };

            ViewData["reffer"] = Request.Headers["Referer"].ToString();
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
            /*******************************************************************************************************
            *      GET: ForumPosts/Edit/3
            *      Returns the ForumPost edit view. 
            *******************************************************************************************************/

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
            ViewData["reffer"] = Request.Headers["Referer"].ToString();
            return View(forumPost);
        }

        // POST: ForumPosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ForumText,CreationDate,UserId,ForumTopicId, LikeCount")] ForumPost forumPost)
        {
            /*******************************************************************************************************
            *      POST: ForumPosts/Edit/3
            *      Updates the post in database.
            *******************************************************************************************************/

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
                return RedirectToAction("Details", new {id});
            }
            ViewData["ForumTopicId"] = new SelectList(_context.ForumTopics, "Id", "Id", forumPost.ForumTopicId);
            return View(forumPost);
        }

        // GET: ForumPosts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            /**********************************************************************************************************************************
            *      GET: ForumPosts/Delete/3
            *      Checks is id exists. Verifies the user trying to delete post is same as post. Returns DeleteConfirmed View
            ***********************************************************************************************************************************/

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
            else if (forumPost.UserId != _userManager.GetUserId(HttpContext.User))
            {
                return RedirectToAction("Details", "ForumPosts", new {id = forumPost.Id});
            }

            return View(forumPost);
        }

        // POST: ForumPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            /**********************************************************************************************************************************
            *      POST: ForumPosts/Delete/3
            *      Deletes post from database.
            ***********************************************************************************************************************************/

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
