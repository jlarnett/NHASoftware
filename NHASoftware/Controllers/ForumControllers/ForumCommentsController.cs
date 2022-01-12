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

namespace NHASoftware.Controllers
{
    public class ForumCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForumCommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            /*******************************************************************************************************
            *      GET: ForumComments/details/5
            *      Returns Comment detail view. Not implemented anywhere. Might still work.
            *******************************************************************************************************/

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

        [Authorize]
        public IActionResult Create(int id)
        {
            /*******************************************************************************************************
            *      GET: ForumComments/Create
            *      Returns Comment Create View. Populates some server side information. 
            *******************************************************************************************************/

            var comment = new ForumComment()
            {
                CreationDate = DateTime.Now,
                UserId = _userManager.GetUserId(HttpContext.User),
                ForumPostId = id
            };

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentText,CreationDate,UserId,ForumPostId")] ForumComment forumComment)
        {
            /*******************************************************************************************************
            *      Post: ForumComments/Create
            *      Create comment. rechecks postCount ^ commentCount.
            *******************************************************************************************************/

            if (ModelState.IsValid)
            {
                //Gets total comments for post & increments it. 
                int commentCount =  _context.ForumComments.Count(c=> c.ForumPostId == forumComment.ForumPostId);
                var post = await _context.ForumPost.FindAsync(forumComment.ForumPostId);
                post.CommentCount = commentCount +1;


                //Checking topic postCount & updating latest post.
                int postThreads = _context.ForumPost.Count(c => c.ForumTopicId == post.ForumTopicId);
                int postComments = _context.ForumComments.Count(c => c.ForumPost.ForumTopicId == post.ForumTopicId);
                var total = postThreads + postComments;

                var postTopic = await _context.ForumTopics.FindAsync(post.ForumTopicId);
                postTopic.PostCount = total;
                postTopic.LastestPost = DateTime.Now;

                _context.Add(forumComment);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            return View(forumComment);
        }

        // GET: ForumComments/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            /*******************************************************************************************************
            *      GET: ForumComments/Edit
            *      Checks if id exist & returns Comment Edit View
            *******************************************************************************************************/

            if (id == null)
            {
                return NotFound();
            }

            var forumComment = await _context.ForumComments.FindAsync(id);

            if (forumComment == null)
            {
                return NotFound();
            }
            else if (forumComment.UserId != _userManager.GetUserId(HttpContext.User))
            {
                return RedirectToAction("Details", "ForumPosts", new { id = forumComment.ForumPostId });
            }
            ViewData["ForumPostId"] = new SelectList(_context.ForumPost, "Id", "Id", forumComment.ForumPostId);
            return View(forumComment);
        }

        // POST: ForumComments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CommentText,CreationDate,UserId,ForumPostId")] ForumComment forumComment)
        {
            /*******************************************************************************************************
            *      POST: ForumComments/Edit
            *      Updates the comment.
            *******************************************************************************************************/

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
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            /*******************************************************************************************************
            *      GET: ForumComments/Delete/5
            *      Checks comment exists. Returns Delete confirmation page. Verifies user trying to delete matches comment.
            *******************************************************************************************************/

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
            else if (forumComment.UserId != _userManager.GetUserId(HttpContext.User))
            {
                return RedirectToAction("Details", "ForumPosts", new {id = forumComment.ForumPostId});
            }

            return View(forumComment);
        }

        // POST: ForumComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            /*******************************************************************************************************
            *      POST: ForumComments/Delete/5
            *      Deletes the comment from database.
            *******************************************************************************************************/

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
