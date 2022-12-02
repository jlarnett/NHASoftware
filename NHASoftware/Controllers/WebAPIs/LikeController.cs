using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHASoftware.DBContext;
using NHASoftware.Services.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Controllers.WebAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public LikeController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Adds a like to forum post comment. endpoint = api/Like/Comment/id
        /// </summary>
        /// <param name="id">The forum post comment to add a like too. </param>
        /// <returns></returns>
        [HttpPut("Comment/{id}")]
        public async Task<IActionResult> PutLike(int id)
        {
            //return "You accessed the Like API!" + id.ToString();

            var comment = await _unitOfWork.ForumCommentRepository.GetByIdAsync(id);

            if(comment == null)
            {
                return new JsonResult(new { success = false });
            }
            else
            {
                comment.LikeCount ++;
            }

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumCommentExists(id))
                {
                    return new JsonResult(new { success = false });

                }
                else
                {
                    throw;
                }
            }

            return new JsonResult(new { success = true });
        }

        /// <summary>
        /// Adds a like to forum post comment. endpoint = api/Like/Comment/id
        /// </summary>
        /// <param name="id">The forum post comment to add a like too. </param>
        /// <returns></returns>
        [HttpPut("Post/{id}")]
        public async Task<IActionResult> AddLikeToPost(int id)
        {
            var post = await _unitOfWork.ForumPostRepository.GetForumPostWithLazyLoadingAsync(id);

            if(post == null)
            {
                return new JsonResult(new { success = false });
            }
            else
            {
                post.LikeCount ++;
            }

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumPostExists(id))
                {
                    return new JsonResult(new { success = false });

                }
                else
                {
                    throw;
                }
            }

            return new JsonResult(new { success = true });
        }

        private bool ForumCommentExists(int id)
        {
            return _context.ForumComments.Any(e => e.Id == id);
        }

        private bool ForumPostExists(int id)
        {
            return _context.ForumPosts.Any(e => e.Id == id);
        }
    }
}
