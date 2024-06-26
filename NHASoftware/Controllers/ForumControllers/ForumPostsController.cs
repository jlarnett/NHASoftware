﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement.Mvc;
using NHA.Helpers.HtmlStringCleaner;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.AccessWarden;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Views.ViewModels.ForumVMs;
namespace NHA.Website.Software.Controllers.ForumControllers;
[FeatureGate("ForumsEnabled")]
public class ForumPostsController : Controller
{
    //DI Services
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWarden _accessWarden;
    private readonly IHtmlStringCleaner _htmlbuilder;
    private readonly IUnitOfWork _unitOfWork;

    public ForumPostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        IWarden accessWarden, IHtmlStringCleaner htmlbuilder, IUnitOfWork unitOfWork)
    {
        _context = context;
        _userManager = userManager;
        _accessWarden = accessWarden;
        _htmlbuilder = htmlbuilder;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// GET: ForumPosts/Details/5
    /// Returns forumPost Details view for the supplied id. 
    /// </summary>
    /// <param name="id">The forumId you want to view the details page of.</param>
    /// <returns></returns>
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var forumPost = await _unitOfWork.ForumPostRepository.GetForumPostWithLazyLoadingAsync(id);

        if (forumPost == null)
        {
            return NotFound();
        }

        forumPost.ForumText = _htmlbuilder.Clean(forumPost.ForumText);
        var comments = await _unitOfWork.ForumCommentRepository.GetForumPostCommentsAsync(id);

        foreach (var comment in comments)
        {
            comment.CommentText = _htmlbuilder.Clean(comment.CommentText);
        }

        var detailVm = new ForumPostDetailModel()
        {
            ForumPost = forumPost,
            ForumComments = comments
        };

        return View(detailVm);
    }

    /// <summary>
    /// GET: ForumPosts/Create
    /// Populates some server side information for post. 
    /// Returns the Create forumPost View.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize]
    public IActionResult Create(int id)
    {
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

    /// <summary>
    /// POST: ForumPosts/Create
    /// Adds forumPost to the database if the model is valid.
    /// Increments the Post Count & Thread Count & LatestPost.
    /// </summary>
    /// <param name="forumPost">ForumPost object to bind values too</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,ForumText,CreationDate, UserId, ForumTopicId, CommentCount")] ForumPost forumPost)
    {
        if (ModelState.IsValid)
        {
            var topic = await _unitOfWork.ForumTopicRepository.GetByIdAsync(forumPost.ForumTopicId);

            if (topic != null)
            {
                topic.PostCount += 1;
                topic.ThreadCount += 1;
                topic.LastestPost = DateTime.Now;
            }

            _unitOfWork.ForumPostRepository.Add(forumPost);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Details", "ForumTopics", new { id = forumPost.ForumTopicId });
        }
        return View(forumPost);
    }

    /// <summary>
    /// GET: ForumPosts/Edit/5
    /// Checks if the id parameter forumPost exist. Returns the forumPost edit view.
    /// </summary>
    /// <param name="id">id of the forumPost to be edited. </param>
    /// <returns></returns>
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

        var forumPost = await _unitOfWork.ForumPostRepository.GetForumPostWithLazyLoadingAsync(id);

        if (forumPost == null)
        {
            return NotFound();
        }

        if (forumPost.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) || IsUserForumAdmin())
        {
            ViewData["ForumTopicId"] = new SelectList(await _unitOfWork.ForumTopicRepository.GetAllAsync(), "Id", "Title", forumPost.ForumTopicId);
            ViewData["reffer"] = Request.Headers["Referer"].ToString();
            return View(forumPost);
        }
        else
        {
            return Unauthorized();
        }
    }

    /// <summary>
    /// POST: ForumPosts/Edit/5
    /// Updates the forumPost in database.
    /// </summary>
    /// <param name="id">forumPost Id</param>
    /// <param name="forumPost">Post object to bind form values too</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ForumText,CreationDate,UserId,ForumTopicId, LikeCount")] ForumPost forumPost)
    {
        if (id != forumPost.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == forumPost.UserId || IsUserForumAdmin())
            {
                try
                {
                    _unitOfWork.ForumPostRepository.Update(forumPost);
                    await _unitOfWork.CompleteAsync();
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
                return RedirectToAction("Details", new { id });
            }
        }
        ViewData["ForumTopicId"] = new SelectList(await _unitOfWork.ForumTopicRepository.GetAllAsync(), "Id", "Id", forumPost.ForumTopicId);
        return View(forumPost);
    }

    /// <summary>
    /// GET: ForumPosts/Delete/5
    /// Checks if the id exist. Verifies the user trying to delete post is admin or associated user. Returns DeleteConfirmed View.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var forumPost = await _unitOfWork.ForumPostRepository.GetForumPostWithLazyLoadingAsync(id);

        if (forumPost == null)
        {
            return NotFound();
        }

        if (forumPost.UserId != _userManager.GetUserId(HttpContext.User) && !IsUserForumAdmin())
        {
            return RedirectToAction("Details", "ForumPosts", new { id = forumPost.Id });
        }

        ViewData["reffer"] = Request.Headers["Referer"].ToString();
        return View(forumPost);
    }

    /// <summary>
    /// POST: ForumPosts/Delete/5
    /// Deletes the ForumPost form database.
    /// </summary>
    /// <param name="id">forumPost id to delete</param>
    /// <returns></returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var forumPost = await _unitOfWork.ForumPostRepository.GetForumPostWithLazyLoadingAsync(id);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (forumPost != null)
        {
            if (userId == forumPost.UserId || IsUserForumAdmin())
            {
                var topic = await _unitOfWork.ForumTopicRepository.GetByIdAsync(forumPost.ForumTopicId);
                var postCommentsNumber = await _unitOfWork.ForumCommentRepository.GetNumberOfCommentsForPost(forumPost.Id);

                if (topic != null)
                {
                    topic.PostCount -= postCommentsNumber + 1;
                    topic.ThreadCount -= 1;
                    topic.LastestPost = DateTime.Now;
                }

                var oldPostTopicId = forumPost.ForumTopicId;

                _unitOfWork.ForumPostRepository.Remove(forumPost);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Details", "ForumTopics", new { id = oldPostTopicId });
            }
            else
            {
                return Unauthorized();
            }
        }
        else
        {
            return NotFound();
        }
    }


    /// <summary>
    /// Checks if the forumPost exist using the id parameter
    /// </summary>
    /// <param name="id">ForumPost id to check existence</param>
    /// <returns>Returns true if ForumPost exist.</returns>
    private bool ForumPostExists(int id)
    {
        return _context.ForumPosts!.Any(e => e.Id == id);
    }

    /// <summary>
    /// Checks if the current logged in user is admin or forum admin
    /// </summary>
    /// <returns>Returns Bool if logged in user IS admin or forum admin</returns>
    private bool IsUserForumAdmin()
    {
        return _accessWarden.IsForumAdmin(User);
    }
}
