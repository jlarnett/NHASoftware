using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.ConsumableEntities;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Services.CacheGoblin;
using NHA.Website.Software.Services.CookieMonster;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Controllers.WebAPIs;
[Route("api/[controller]")]
[ApiController]
public class LikeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICookieMonster _cookieMonster;
    private readonly ICacheGoblin<CookieTrackingObject> _commentCacheGoblin;
    private readonly ICacheGoblin<CookieTrackingObject> _postCacheGoblin;
    private readonly ICacheGoblin<CookieTrackingObject> _animeCacheGoblin;
    private readonly ICacheGoblin<CookieTrackingObject> _gameCacheGoblin;

    public LikeController(ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ICookieMonster cookieMonster,
        ICacheGoblin<CookieTrackingObject> commentCacheGoblin,
        ICacheGoblin<CookieTrackingObject> postCacheGoblin,
        ICacheGoblin<CookieTrackingObject> animeCacheGoblin,
        ICacheGoblin<CookieTrackingObject> gameCacheGoblin)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _cookieMonster = cookieMonster;
        _commentCacheGoblin = commentCacheGoblin;
        _postCacheGoblin = postCacheGoblin;
        _animeCacheGoblin = animeCacheGoblin;
        _gameCacheGoblin = gameCacheGoblin;
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

        if (!currentSessionId.Equals(string.Empty))
        {
            var user = new CookieTrackingObject(id, currentSessionId);

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
            _commentCacheGoblin.Add(new CookieTrackingObject(id, _cookieMonster.TryRetrieveCookie(CookieKeys.Session)));
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

        if (!currentSessionId.Equals(string.Empty))
        {
            var user = new CookieTrackingObject(id, currentSessionId);

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

            _postCacheGoblin.Add(new CookieTrackingObject(id, _cookieMonster.TryRetrieveCookie(CookieKeys.Session)));
            return await TryIncrementPostLikes(id);
        }
    }

    [HttpPut("Anime/{id}")]
    public async Task<IActionResult> TryLikeAnime(int id, [FromQuery] bool upvote)
    {
        var currentSessionId = _cookieMonster.TryRetrieveCookie(CookieKeys.Session);

        if (!currentSessionId.Equals(string.Empty))
        {
            var user = new CookieTrackingObject(id, currentSessionId);
            var animeLikeExists = _animeCacheGoblin.Exists(user);

            if (animeLikeExists)
            {
                return new JsonResult(new { success = false });
            }
            else
            {
                _animeCacheGoblin.Add(user);
                return await TryModifyAnimeLikes(id, upvote);
            }
        }
        else
        {
            _cookieMonster.CreateCookie(CookieKeys.Session, Guid.NewGuid().ToString());
            _animeCacheGoblin.Add(new CookieTrackingObject(id, _cookieMonster.TryRetrieveCookie(CookieKeys.Session)));
            return await TryModifyAnimeLikes(id, upvote);
        }
    }

    [HttpPut("Game/{id}")]
    public async Task<IActionResult> TryLikeGame(int id, [FromQuery] bool upvote)
    {
        var currentSessionId = _cookieMonster.TryRetrieveCookie(CookieKeys.Session);

        if (!currentSessionId.Equals(string.Empty))
        {
            var user = new CookieTrackingObject(id, currentSessionId);
            var gameLikeExists = _gameCacheGoblin.Exists(user);

            if (gameLikeExists)
            {
                return new JsonResult(new { success = false });
            }
            else
            {
                _gameCacheGoblin.Add(user);
                return await TryModifyGameLikes(id, upvote);
            }
        }
        else
        {
            _cookieMonster.CreateCookie(CookieKeys.Session, Guid.NewGuid().ToString());
            _gameCacheGoblin.Add(new CookieTrackingObject(id, _cookieMonster.TryRetrieveCookie(CookieKeys.Session)));
            return await TryModifyGameLikes(id, upvote);
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

    private bool AnimePageExists(int id)
    {
        return _context.AnimePages!.Any(e => e.Id == id);
    }

    private bool GamePageExists(int id)
    {
        return _context.GamePages!.Any(e => e.Id == id);
    }

    private async Task<IActionResult> TryModifyAnimeLikes(int id, bool islike = false)
    {
        var animePage = await _unitOfWork.AnimePageRepository.GetByIdAsync(id);

        if (animePage == null)
           return new JsonResult(new { success = false });

        if (islike)
            animePage.UpVotes++;
        else
            animePage.DownVotes++;

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnimePageExists(id))
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

    private async Task<IActionResult> TryModifyGameLikes(int id, bool islike = false)
    {
        var gamePage = await _unitOfWork.GamePageRepository.GetByIdAsync(id);

        if (gamePage == null)
            return new JsonResult(new { success = false });

        if (islike)
            gamePage.UpVotes++;
        else
            gamePage.DownVotes++;

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GamePageExists(id))
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
