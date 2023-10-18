#nullable disable
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement.Mvc;
using NHAHelpers.HtmlStringCleaner;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;
using NHASoftware.Services.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    [FeatureGate("ForumsEnabled")]
    public class ForumTopicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlStringCleaner _htmlCleaner;
        private readonly IUnitOfWork _unitOfWork;

        public ForumTopicsController(ApplicationDbContext context, IHtmlStringCleaner htmlCleaner, IUnitOfWork unitOfWork)
        {
            _context = context;
            _htmlCleaner = htmlCleaner;
            _unitOfWork = unitOfWork;
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

            var forumTopic = await _unitOfWork.ForumTopicRepository.GetByIdAsync(id);

            if (forumTopic == null)
            {
                return NotFound();
            }

            var topicPosts = await _unitOfWork.ForumTopicRepository.GetForumTopicPostsAsync(id);

            foreach (var post in topicPosts)
            {
                post.ForumText = _htmlCleaner.Clean(post.ForumText);
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
        public async Task<IActionResult> Create()
        {
            ViewData["ForumSectionId"] = new SelectList(await _unitOfWork.ForumSectionRepository.GetAllAsync(), "Id", "Name");

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
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ForumSectionId,PostCount,ThreadCount")] ForumTopic forumTopic)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ForumTopicRepository.Add(forumTopic);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Index", "Forum");
            }

            ViewData["ForumSectionId"] = new SelectList(await _unitOfWork.ForumSectionRepository.GetAllAsync(), "Id", "Name", forumTopic.ForumSectionId);
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

            var forumTopic = await _unitOfWork.ForumTopicRepository.GetByIdAsync(id);

            if (forumTopic == null)
            {
                return NotFound();
            }

            ViewData["reffer"] = Request.Headers["Referer"].ToString();
            ViewData["ForumSectionId"] = new SelectList(await _unitOfWork.ForumSectionRepository.GetAllAsync(), "Id", "Name", forumTopic.ForumSectionId);
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
                    _unitOfWork.ForumTopicRepository.Update(forumTopic);
                    await _unitOfWork.CompleteAsync();
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
            ViewData["ForumSectionId"] = new SelectList(await _unitOfWork.ForumSectionRepository.GetAllAsync(), "Id", "Name", forumTopic.ForumSectionId);
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

            var forumTopic = await _unitOfWork.ForumTopicRepository.GetByIdAsync(id);

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
            var forumTopic = await _unitOfWork.ForumTopicRepository.GetByIdAsync(id);
            _unitOfWork.ForumTopicRepository.Remove(forumTopic);
            await _unitOfWork.CompleteAsync();

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
