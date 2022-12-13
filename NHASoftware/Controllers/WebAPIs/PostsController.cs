using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHASoftware.ConsumableEntities.DTOs;
using NHASoftware.DBContext;
using NHASoftware.Entities;
using NHASoftware.Entities.Social_Entities;

namespace NHASoftware.Controllers.WebAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PostsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this._mapper = mapper;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<IEnumerable<PostDTO>> GetPosts()
        {
            var posts = await _context.Posts.Include(p => p.User).ToListAsync();
            var postsDto = posts.Select((_mapper.Map<Post, PostDTO>));
            return postsDto;
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int? id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int? id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        /// <summary>
        /// POST: api/Posts
        /// API Endpoint for creating new social media post.
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> PostPost([Bind("Summary,UserId,ParentPostId")] Post post)
        {
            //var result = _context.Posts.FromSqlRaw("posts_InsertPostData", post.Summary, DateTime.Today, post.ParentPostId);
            _context.Posts.Add(post);
            var result = await _context.SaveChangesAsync();
            return result > 0 ? new JsonResult(new { success = true }) : new JsonResult(new { success = false });
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int? id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
