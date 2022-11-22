using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;

namespace NHASoftware.Controllers.WebAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumCommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ForumCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/ForumComments
        /// Returns a list of all forumComments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumComment>>> GetForumComments()
        { 
            if (_context.ForumComments == null)
            { 
                return NotFound();
            }

            return await _context.ForumComments.ToListAsync();
        }

        /// <summary>
        /// GET: api/ForumComments/5
        /// Returns the forum comment of the supplied id. 
        /// </summary>
        /// <param name="id">id of the forum comment you want details of. </param>
        /// <returns>forum comment</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ForumComment>> GetForumComment(int id)
        {
          
            if (_context.ForumComments == null)
            {
                return NotFound();
            }
            var forumComment = await _context.ForumComments.FindAsync(id);

            if (forumComment == null)
            {
                return NotFound();
            }

            return forumComment;
        }

        // PUT: api/ForumComments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutForumComment(int id, ForumComment forumComment)
        //{
        //    if (id != forumComment.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(forumComment).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ForumCommentExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/ForumComments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<ForumComment>> PostForumComment(ForumComment forumComment)
        //{
        //      if (_context.ForumComments == null)
        //      {
        //          return Problem("Entity set 'ApplicationDbContext.ForumComments'  is null.");
        //      }
        //      _context.ForumComments.Add(forumComment);
        //      await _context.SaveChangesAsync(); 

        //      return CreatedAtAction("GetForumComment", new { id = forumComment.Id }, forumComment);
        //}

        /// <summary>
        /// DELETE: api/ForumComments/5
        /// Deletes the forum Comment with the supplied id. 
        /// </summary>
        /// <param name="id">id of the forum comment you want removed from database. </param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<JsonResult> DeleteForumComment(int id)
        {
            if (_context.ForumComments == null)
            {
                return new JsonResult(new { success = false });
            }

            var forumComment = await _context.ForumComments.FindAsync(id);

            if (forumComment == null)
            {
                return new JsonResult(new { success = false });
            }

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) == forumComment.UserId || IsUserForumAdmin())
            {
                var post = _context.ForumPosts.FindAsync(forumComment.ForumPostId);

                if (post.Result != null)
                {
                    var topic = _context.ForumTopics.FindAsync(post.Result.ForumTopicId);
                    post.Result.CommentCount--;

                    if (topic.Result != null)
                    {
                        topic.Result.PostCount--;
                    }
                }

                _context.ForumComments.Remove(forumComment);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            else
            {
                return new JsonResult(new { success = false });
            }
        }

        /// <summary>
        /// Checks if the supplied ids forum Comment exist in the database.
        /// </summary>
        /// <param name="id">id of the comment you want to check existence of. </param>
        /// <returns></returns>
        private bool ForumCommentExists(int id)
        {
            return (_context.ForumComments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
        /// <summary>
        /// Checks whether the current user is a admin or forum admin. 
        /// </summary>
        /// <returns>Returns BOOL True if current user is a forum admin or higher. </returns>
        private bool IsUserForumAdmin()
        {
            if (User.IsInRole("admin") || User.IsInRole("forum admin"))
            {
                return true;
            }

            return false;
        }
    }
}
