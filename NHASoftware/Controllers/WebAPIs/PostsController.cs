using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHASoftware.ConsumableEntities.DTOs;
using NHASoftware.DBContext;
using NHASoftware.Entities;
using NHASoftware.Entities.Social_Entities;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Controllers.WebAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PostsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this._mapper = mapper;
            this._unitOfWork = unitOfWork;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<IEnumerable<PostDTO>> GetPosts()
        {
            var posts = await _unitOfWork.PostRepository.GetAllPostsWithIncludesAsync();
            var postsDtos = posts.Select((_mapper.Map<Post, PostDTO>));
            return postsDtos;
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int? id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int? id, PostDTO postDto)
        {
            var post =_mapper.Map<PostDTO, Post>(postDto);

            if (id != post.Id)
            {
                return BadRequest();
            }

            _unitOfWork.PostRepository.Update(post);

            try
            {
                await _unitOfWork.CompleteAsync();
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
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<JsonResult> PostPost([Bind("Summary,UserId,ParentPostId")] PostDTO postdto)
        {
            var post = _mapper.Map<PostDTO, Post>(postdto);
            post.CreationDate = DateTime.Now;
            _unitOfWork.PostRepository.Add(post);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? new JsonResult(new { success = true }) : new JsonResult(new { success = false });
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            _unitOfWork.PostRepository.Remove(post);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0 ? NoContent() : BadRequest();
        }

        private bool PostExists(int? id)
        {
            var post = _unitOfWork.PostRepository.Find(p => p.Id.Equals(id));
            return post.Any();
        }
    }
}
