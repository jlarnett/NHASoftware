using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.Caching;
using NHA.Website.Software.Services.CacheLoadingManager;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.Entities.Social_Entities;

namespace NHA.Website.Software.Controllers.WebAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class GratitudeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheLoadingManager _cacheLoadingManager;

        public GratitudeController(IUnitOfWork unitOfWork, ICacheLoadingManager cacheLoadingManager)
        {
            _unitOfWork = unitOfWork;
            _cacheLoadingManager = cacheLoadingManager;
        }

        [HttpPost("Userlike")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikePost([Bind("UserId, PostId", "IsDislike")] UserLikes userlike)
        {
            if (ModelState.IsValid && !UserLikeExists(userlike))
            {
                _unitOfWork.UserLikeRepository.Add(userlike);
                var rowsAltered = await _unitOfWork.CompleteAsync();

                if (rowsAltered > 0)
                {
                    _cacheLoadingManager.IncrementCacheChangeCounter(CachingKeys.Posts);
                    return new JsonResult(new { success = true });
                }
            }
            return new JsonResult(new { success = false });
        }

        [HttpPost("UserLike/Exists")]
        [ValidateAntiForgeryToken]
        public IActionResult PostLikeExists([Bind("UserId, PostId, IsDislike")] UserLikes userlike)
        {
            if (UserLikeExists(userlike))
            {
                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });
        }

        [HttpDelete("UserLike")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLike([Bind("UserId, PostId, IsDislike")] UserLikes userlike)
        {
            if (UserLikeExists(userlike))
            {
                var like = _unitOfWork.UserLikeRepository.Find(ul =>
                    ul.PostId == userlike.PostId && ul.UserId == userlike.UserId && ul.IsDislike == userlike.IsDislike).FirstOrDefault();

                if (like != null)
                {
                    _unitOfWork.UserLikeRepository.Remove(like);
                    var result = await _unitOfWork.CompleteAsync();


                    if (result > 0)
                    {
                        _cacheLoadingManager.IncrementCacheChangeCounter(CachingKeys.Posts);
                        return new JsonResult(new { success = true });
                    }

                    return new JsonResult(new { success = false });
                }
            }

            return new JsonResult(new { success = false });
        }

        private bool UserLikeExists(UserLikes userlike)
        {
            return _unitOfWork.UserLikeRepository.Find(ul => ul.PostId == userlike.PostId && ul.UserId == userlike.UserId && ul.IsDislike == userlike.IsDislike).Any();
        }


    }
}
