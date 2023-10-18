#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement.Mvc;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;
using NHASoftware.Services.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Controllers
{
    [FeatureGate("ForumsEnabled")]
    public class ForumSectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ForumSectionsController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// GET: ForumSections/Create
        /// Returns the forum section create view.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewData["reffer"] = Request.Headers["Referer"].ToString();
            return View();
        }

        /// <summary>
        /// POST: ForumSections/Create
        /// Creates forum section in database.
        /// </summary>
        /// <param name="forumSection">Object to bind properties too</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ForumSection forumSection)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ForumSectionRepository.Add(forumSection);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Index", "Forum");
            }
            
            return View(forumSection);
        }

        /// <summary>
        /// GET: ForumSections/Edit/5
        /// Returns the forum section Edit View
        /// </summary>
        /// <param name="id">Section Id to edit</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumSection = await _unitOfWork.ForumSectionRepository.GetByIdAsync(id);

            if (forumSection == null)
            {
                return NotFound();
            }

            ViewData["reffer"] = Request.Headers.Referer.ToString();
            return View(forumSection);
        }

        /// <summary>
        /// POST: ForumSections/Edit/5
        /// Updates the forum Section
        /// </summary>
        /// <param name="id">Forum Section Id to update.</param>
        /// <param name="forumSection">Object all properties are binded to</param>
        /// <returns></returns>
        /// To protect from overposting attacks, enable the specific properties you want to bind to.
        /// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ForumSection forumSection)
        {
            if (id != forumSection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.ForumSectionRepository.Update(forumSection);
                    await _unitOfWork.CompleteAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumSectionExists(forumSection.Id))
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
            return View(forumSection);
        }

        /// <summary>
        /// GET: ForumSections/Delete/5
        /// Checks if the forum section id exist & returns the DeleteConfirmed view. 
        /// </summary>
        /// <param name="id">forum section Id. </param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumSection = await _unitOfWork.ForumSectionRepository.GetByIdAsync(id);

            if (forumSection == null)
            {
                return NotFound();
            }

            ViewData["reffer"] = Request.Headers.Referer.ToString();
            return View(forumSection);
        }

        /// <summary>
        /// POST: ForumSections/Delete/5
        /// Deletes the forum section from database using EF. 
        /// </summary>
        /// <param name="id">Forum Section Id to delete</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumSection = await _unitOfWork.ForumSectionRepository.GetByIdAsync(id);
            _unitOfWork.ForumSectionRepository.Remove(forumSection);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Index", "Forum");
        }

        /// <summary>
        /// Checks if the Forum Section Exist.
        /// </summary>
        /// <param name="id">Forum Section Id</param>
        /// <returns>Returns true if the forum section exist. </returns>
        private bool ForumSectionExists(int id)
        {
            return _context.ForumSections.Any(e => e.Id == id);
        }
    }
}
