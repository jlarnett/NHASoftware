using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Services.CacheGoblin;
using NHA.Website.Software.Services.CookieMonster;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.ConsumableEntities;
using NHASoftware.DBContext;

namespace NHA.Website.Software.Controllers.WebAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICookieMonster _cookieMonster;
        private readonly ICacheGoblin<CookieTrackingObject> _commentCacheGoblin;
        private readonly ICacheGoblin<CookieTrackingObject> _postCacheGoblin;

        public LikeController(ApplicationDbContext context, IUnitOfWork unitOfWork, ICookieMonster cookieMonster, ICacheGoblin<CookieTrackingObject> commentCacheGoblin, ICacheGoblin<CookieTrackingObject> postCacheGoblin)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _cookieMonster = cookieMonster;
            _commentCacheGoblin = commentCacheGoblin;
            _postCacheGoblin = postCacheGoblin;
        }


        /// <summary>
        /// Adds a like to forum post comment. endpoint = api/Like/Comment/id
        /// </summary>
        /// <param name="id">The forum post comment to add a like too. </param>
        /// <returns></returns>
        [HttpPut("Comment/{id}")]
        public async Task<IActionResult> TryLikeComment(int id)
        {
            var currentSessionId = _cookieMonster.TryRetrieveCookie(CookieKeys.Session);

            if (currentSessionId != null)
            {
                var user = new CookieTrackingObject
                {
                    CookieGuid = currentSessionId,
                    ObjectIdentifierId = id
                };

                var commentLikeExists = _commentCacheGoblin.Exists(user);

                if (commentLikeExists)
                {
                    return new JsonResult(new { success = false });
                }
                else
                {
                    _commentCacheGoblin.Add(user);
                    return await TryIncrementCommentLikes(id);
                }
            }
            else
            {
                _cookieMonster.CreateCookie(CookieKeys.Session, Guid.NewGuid().ToString());
                _commentCacheGoblin.Add(new CookieTrackingObject()
                {
                    CookieGuid = _cookieMonster.TryRetrieveCookie(CookieKeys.Session),
                    ObjectIdentifierId = id
                });
                return await TryIncrementCommentLikes(id);
            }
        }

        /// <summary>
        /// Adds a like to forum post comment. endpoint = api/Like/Post/id
        /// </summary>
        /// <param name="id">The forum post comment to add a like too. </param>
        /// <returns></returns>
        [HttpPut("Post/{id}")]
        public async Task<IActionResult> TryLikePost(int id)
        {
            var currentSessionId = _cookieMonster.TryRetrieveCookie(CookieKeys.Session);

            if (currentSessionId != null)
            {
                var user = new CookieTrackingObject
                {
                    CookieGuid = currentSessionId,
                    ObjectIdentifierId = id
                };

                var postLikeExists = _postCacheGoblin.Exists(user);

                if (postLikeExists)
                {
                    return new JsonResult(new { success = false });
                }
                else
                {
                    _postCacheGoblin.Add(user);
                    return await TryIncrementPostLikes(id);
                }
            }
            else
            {
                _cookieMonster.CreateCookie(CookieKeys.Session, Guid.NewGuid().ToString());
                _postCacheGoblin.Add(new CookieTrackingObject()
                {
                    CookieGuid = _cookieMonster.TryRetrieveCookie(CookieKeys.Session),
                    ObjectIdentifierId = id
                });
                return await TryIncrementPostLikes(id);
            }
        }
        private bool ForumCommentExists(int id)
        {
            return _context.ForumComments!.Any(e => e.Id == id);
        }

        private bool ForumPostExists(int id)
        {
            return _context.ForumPosts!.Any(e => e.Id == id);
        }

        private async Task<IActionResult> TryIncrementPostLikes(int id)
        {
            var post = await _unitOfWork.ForumPostRepository.GetForumPostWithLazyLoadingAsync(id);

            if (post == null)
            {
                return new JsonResult(new { success = false });
            }
            else
            {
                post.LikeCount++;
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

        private async Task<IActionResult> TryIncrementCommentLikes(int id)
        {
            var comment = await _unitOfWork.ForumCommentRepository.GetByIdAsync(id);

            if (comment == null)
            {
                return new JsonResult(new { success = false });
            }
            else
            {
                comment.LikeCount++;
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
    }
}
