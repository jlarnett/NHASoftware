using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.Models.ForumModels;

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

        // GET: api/ForumComments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumComment>>> GetForumComments()
        {
          if (_context.ForumComments == null)
          {
              return NotFound();
          }
            return await _context.ForumComments.ToListAsync();
        }

        // GET: api/ForumComments/5
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForumComment(int id, ForumComment forumComment)
        {
            if (id != forumComment.Id)
            {
                return BadRequest();
            }

            _context.Entry(forumComment).State = EntityState.Modified;

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

        // POST: api/ForumComments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ForumComment>> PostForumComment(ForumComment forumComment)
        {
          if (_context.ForumComments == null)
          {
              return Problem("Entity set 'ApplicationDbContext.ForumComments'  is null.");
          }
            _context.ForumComments.Add(forumComment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetForumComment", new { id = forumComment.Id }, forumComment);
        }

        // DELETE: api/ForumComments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumComment(int id)
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

            _context.ForumComments.Remove(forumComment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ForumCommentExists(int id)
        {
            return (_context.ForumComments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
