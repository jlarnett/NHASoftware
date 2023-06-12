using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHASoftware.ConsumableEntities.DTOs;
using NHASoftware.Entities.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public PostsController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ILogger<PostDTO> logger)
        {
            this._mapper = mapper;
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
            this._logger = logger;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<IEnumerable<PostDTO>> GetPosts()
        {
            var posts = await _unitOfWork.PostRepository.GetAllPostsWithIncludesAsync();
            var postsDtos = posts.Select((_mapper.Map<Post, PostDTO>)).ToList();
            return await PopulatePostDTOLikeDetails(postsDtos);
        }

        /// <summary>
        /// Gets all social posts for supplied userId. Populates the PostDTO like details. 
        /// </summary>
        /// <param name="userId">Users Identity Id you want posts for</param>
        /// <returns>postsDto IEnumerable </returns>
        [HttpGet("GetSocialPostForUserId/{userId}")]
        public async Task<IEnumerable<PostDTO>> GetAllPostForUserId(string userId)
        {
            var posts = await _unitOfWork.PostRepository.GetUsersSocialPostsAsync(userId);
            var postsDtos = posts.Select((_mapper.Map<Post, PostDTO>)).ToList();
            return await PopulatePostDTOLikeDetails(postsDtos);
        }

        // GET: api/Posts/5
        [HttpGet("GetSocialPosts/{id}")]
        public async Task<ActionResult<Post>> GetPost(int? id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpGet("FindChildrenPosts/{id}")]
        public async Task<IEnumerable<PostDTO>> FindChildrenPosts(int? id)
        {
            var posts = await _unitOfWork.PostRepository.GetAllPostsWithIncludesAsync();
            var childrenPosts = posts.Where(p => p.ParentPostId == id).ToList();

            var postsDtos = childrenPosts.Select((_mapper.Map<Post, PostDTO>)).ToList();
            return await PopulatePostDTOLikeDetails(postsDtos);
        }

        [HttpGet("GetUserSocialPosts/{id}")]
        public async Task<List<PostDTO>> GetUsersSocialPosts(string id)
        {
            var posts = await _unitOfWork.PostRepository.GetUsersSocialPostsAsync(id);
            return posts.Select((_mapper.Map<Post, PostDTO>)).ToList();
        } 

        private async Task<IEnumerable<PostDTO>> PopulatePostDTOLikeDetails(List<PostDTO> postDtos)
        {
            List<PostDTO> posts = new List<PostDTO>();
            foreach (var dto in postDtos)
            {
                posts.Add(new PostDTO()
                {
                    CreationDate = dto.CreationDate,
                    DislikeCount = _unitOfWork.UserLikeRepository.Find(p => p.PostId == dto.Id && p.IsDislike).Count(),
                    UserLikedPost = UserLikedPost(dto.Id),
                    UserDislikedPost = UserDislikedPost(dto.Id),
                    UserId = dto.UserId,
                    User = dto.User,
                    Id = dto.Id,
                    LikeCount = _unitOfWork.UserLikeRepository.Find(p => p.PostId == dto.Id && !p.IsDislike).Count(),
                    ParentPostId = dto.ParentPostId,
                    ParentPost = dto.ParentPost,
                    Summary = dto.Summary
                });
            }
            return posts.AsEnumerable();
        }

        /// <summary>
        /// Checks whether the current user has liked the specified Post Id. 
        /// </summary>
        /// <param name="id">Post Id</param>
        /// <returns>Returns boolean condition if user liked post id</returns>
        private bool UserLikedPost(int? id)
        {
            return _unitOfWork.UserLikeRepository.Find(ul =>
                ul.PostId == id && ul.IsDislike == false && ul.UserId == _userManager.GetUserId(User)).Any();
        }

        /// <summary>
        /// Checks whether the current user has Disliked the specified Post Id. 
        /// </summary>
        /// <param name="id">Post Id</param>
        /// <returns>Returns boolean condition if user Disliked post id</returns>
        private bool UserDislikedPost(int? id)
        {
            return _unitOfWork.UserLikeRepository.Find(ul =>
                ul.PostId == id && ul.IsDislike == true && ul.UserId == _userManager.GetUserId(User)).Any();
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

        // DELETE: api/Posts/5ARM
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
