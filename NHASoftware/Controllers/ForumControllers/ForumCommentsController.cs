#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.HelperClasses;
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

        /// <summary>
        /// GET: ForumComments
        /// Gets the ForumComments index page. Gets all comments to list. 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumComments.Include(f => f.ForumPost);
            return View(await applicationDbContext.ToListAsync());
       }

        /// <summary>
        /// GET: ForumComments/Details/5
        /// returns forum comment detail view. Not Implemented anywhere. Might still work. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// GET: ForumComments/Create/5
        /// Returns the Comment Create View. Populates some server side information. 
        /// </summary>
        /// <param name="id">Forum Post Id.</param>
        /// <returns></returns>
        [Authorize]
        public IActionResult Create(int id)
        {
            var comment = new ForumComment()
            {
                CreationDate = DateTime.Now,
                UserId = _userManager.GetUserId(HttpContext.User),
                ForumPostId = id
            };

            ViewData["reffer"] = Request.Headers["Referer"].ToString();
            return View(comment);
        }

        /// <summary>
        /// POST: ForumComments/Create
        /// Creates a new forum comment. Post to the database using Entity framework. 
        /// </summary>
        /// <param name="forumComment">The variable all incoming properties are binded to</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentText,CreationDate,UserId,ForumPostId")] ForumComment forumComment)
        {
            if (ModelState.IsValid)
            {
                //Gets total comments for post & increments it. 
                var post = await _context.ForumPost.FindAsync(forumComment.ForumPostId);

                if (post != null)
                {
                    post.CommentCount++;
                }

                var postTopic = await _context.ForumTopics.FindAsync(post.ForumTopicId);

                if (postTopic != null)
                {
                    postTopic.PostCount++;
                    postTopic.LastestPost = DateTime.Now;
                }

                _context.Add(forumComment);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Details", "ForumPosts", new {id=forumComment.ForumPostId});
            }
            return View(forumComment);
        }

        /// <summary>
        /// GET: ForumComments/Edit/5
        /// Checks if the id exist & returns the forumComment edit view. 
        /// </summary>
        /// <param name="id">Comment Id you want to attempt to edit. </param>
        /// <returns></returns>
        [Authorize]
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

            if (forumComment.UserId != _userManager.GetUserId(HttpContext.User) && !IsUserForumAdmin())
            {
                return RedirectToAction("Details", "ForumPosts", new { id = forumComment.ForumPostId });
            }
            ViewData["ForumPostId"] = new SelectList(_context.ForumPost, "Id", "Id", forumComment.ForumPostId);
            ViewData["reffer"] = Request.Headers["Referer"].ToString();
            return View(forumComment);
        }

        /// <summary>
        /// POST: ForumComments/Edit/5
        /// Updates forum comment. 
        /// </summary>
        /// <param name="id">comment ID</param>
        /// <param name="forumComment">forumComment that properties are binded too.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CommentText,CreationDate,UserId,ForumPostId,LikeCount")] ForumComment forumComment)
        {

            if (id != forumComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (forumComment.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) || IsUserForumAdmin())
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
                    return RedirectToAction("Details", "ForumPosts", new {id = forumComment.ForumPostId});
                }
                else
                {
                    return Unauthorized();
                }
            }
            ViewData["ForumPostId"] = new SelectList(_context.ForumPost, "Id", "Id", forumComment.ForumPostId);
            return View(forumComment);
        }

        /// <summary>
        /// GET: Return ForumComments Delete Confirmed page. Verifies user trying to delete matches comment
        /// ForumComments/Delete/5
        /// </summary>
        /// <param name="id">Forum Comment ID.</param>
        /// <returns></returns>
        [Authorize]
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

            if (forumComment.UserId != _userManager.GetUserId(HttpContext.User) && !IsUserForumAdmin())
            {
                return RedirectToAction("Details", "ForumPosts", new {id = forumComment.ForumPostId});
            }

            return View(forumComment);
        }

        /// <summary>
        /// POST Method Deletes the forum comment: ForumComments/Delete/5
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var forumComment = await _context.ForumComments.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (forumComment != null)
            {
                if (userId == forumComment.UserId || IsUserForumAdmin())
                {
                    _context.ForumComments.Remove(forumComment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "ForumPosts", new {id=forumComment.ForumPostId});
                }
                else
                {
                    return Unauthorized();
                }
            }

            return NotFound();
        }

        /// <summary>
        /// Checks whether forum comment exists. 
        /// </summary>
        /// <param name="id">Comment Id to check database for</param>
        /// <returns>returns true if comment exists</returns>
        private bool ForumCommentExists(int id)
        {
            return _context.ForumComments.Any(e => e.Id == id);
        }

        /// <summary>
        /// Checks if the User role is either an admin or forum admin. 
        /// </summary>
        /// <returns>returns true if user is admin or forum admin</returns>
        private bool IsUserForumAdmin()
        {
            return PermissionChecker.instance.IsUserForumAdmin(User);
        }
    }
}
