using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;

namespace NHASoftware.Controllers.WebAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LikeController(ApplicationDbContext context)
        {
            _context = context;
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

            var comment = await _context.ForumComments.FindAsync(id);

            if(comment == null)
            {
                return BadRequest();
            }
            else
            {
                comment.LikeCount ++;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumCommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ForumCommentExists(int id)
        {
            return _context.ForumComments.Any(e => e.Id == id);
        }
    }
}
