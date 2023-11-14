#nullable disable
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement.Mvc;
using NHA.Website.Software.Services.AccessWarden;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;
using NHASoftware.Entities.Identity;

namespace NHASoftware.Controllers
{
    [FeatureGate("ForumsEnabled")]
    public class ForumCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWarden _accessWarden;
        private readonly IUnitOfWork _unitOfWork;

        public ForumCommentsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWarden accessWarden,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            _userManager = userManager;
            _accessWarden = accessWarden;
            _unitOfWork = unitOfWork;
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

            var forumComment = await _unitOfWork.ForumCommentRepository.GetForumCommentWithLazyLoadingAsync(id);

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
                var post = await _unitOfWork.ForumPostRepository.GetByIdAsync(forumComment.ForumPostId);

                if (post != null)
                {
                    post.CommentCount++;
                }

                var postTopic = await _unitOfWork.ForumTopicRepository.GetByIdAsync(post.ForumTopicId);

                if (postTopic != null)
                {
                    postTopic.PostCount++;
                    postTopic.LastestPost = DateTime.Now;
                }

                _unitOfWork.ForumCommentRepository.Add(forumComment);
                await _unitOfWork.CompleteAsync();

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

            var forumComment = await _unitOfWork.ForumCommentRepository.GetForumCommentWithLazyLoadingAsync(id);

            if (forumComment == null)
            {
                return NotFound();
            }

            if (forumComment.UserId != _userManager.GetUserId(HttpContext.User) && !IsUserForumAdmin())
            {
                return RedirectToAction("Details", "ForumPosts", new { id = forumComment.ForumPostId });
            }
            ViewData["ForumPostId"] = new SelectList(await _unitOfWork.ForumPostRepository.GetAllAsync(), "Id", "Id", forumComment.ForumPostId);
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
                        _unitOfWork.ForumCommentRepository.Update(forumComment);
                        await _unitOfWork.CompleteAsync();
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
            ViewData["ForumPostId"] = new SelectList(await _unitOfWork.ForumPostRepository.GetAllAsync(), "Id", "Id", forumComment.ForumPostId);
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

            var forumComment = await _unitOfWork.ForumCommentRepository.GetForumCommentWithLazyLoadingAsync(id);

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

            var forumComment = await _unitOfWork.ForumCommentRepository.GetByIdAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (forumComment != null)
            {
                if (userId == forumComment.UserId || IsUserForumAdmin())
                {
                    _unitOfWork.ForumCommentRepository.Remove(forumComment);
                    await _unitOfWork.CompleteAsync();
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
            return _accessWarden.IsForumAdmin(User);
        }
    }
}
