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

        /// <summary>
        /// GET: api/Post
        /// calls the PostRepository and gets all social media posts in DB. DOES NOT INCLUDE DELETE POST. Seperate endpoint for deleted post. 
        /// </summary>
        /// <returns>IEnumerable of all posts.</returns>
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
            var childrenPosts = posts.Where(p => p.ParentPostId == id && p.IsHiddenFromUserProfile == false).ToList();

            var postsDtos = childrenPosts.Select((_mapper.Map<Post, PostDTO>)).ToList();
            return await PopulatePostDTOLikeDetails(postsDtos);
        }

        [HttpGet("GetUserSocialPosts/{id}")]
        public async Task<List<PostDTO>> GetUsersSocialPosts(string id)
        {
            var posts = await _unitOfWork.PostRepository.GetUsersSocialPostsAsync(id);
            return posts.Select((_mapper.Map<Post, PostDTO>)).ToList();
        } 

        /// <summary>
        /// Goes through the entire list of post & repopulates the postDtos with like counters. 
        /// </summary>
        /// <param name="postDtos">List of postsDto objects.</param>
        /// <returns>IEnumerable list of postDtos with the like counters populated. </returns>
        private async Task<IEnumerable<PostDTO>> PopulatePostDTOLikeDetails(List<PostDTO> postDtos)
        {
            List<PostDTO> posts = new List<PostDTO>();
            foreach (var dto in postDtos)
            {
                posts.Add(PopulatePostDTO(dto));
            }
            return posts.AsEnumerable();
        }

        private PostDTO PopulatePostDTO(PostDTO dto)
        {
            return new PostDTO()
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
                Summary = dto.Summary,
            };
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
        /// <param name="postdto"></param>
        /// <returns>Returns IActionResult with new post. </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PostPost([Bind("Summary,UserId,ParentPostId")] PostDTO postdto)
        {
            var post = _mapper.Map<PostDTO, Post>(postdto);
            post.CreationDate = DateTime.Now;

            _unitOfWork.PostRepository.Add(post);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                var newPost = _unitOfWork.PostRepository.Find(p =>
                    p.Summary.Equals(postdto.Summary) && p.UserId.Equals(postdto.UserId)).FirstOrDefault();

                //Populating the postDto
                var newPostWithIncludes = await _unitOfWork.PostRepository.GetPostByIDWithIncludesAsync(newPost.Id.GetValueOrDefault());
                var postDto = PopulatePostDTO(_mapper.Map<Post, PostDTO>(newPostWithIncludes));
                _logger.Log(LogLevel.Information, "Post API successfully added new post to DB {post}", postDto);
                return Ok(new { success = true, data = postDto });
            }
            else
            {
                _logger.Log(LogLevel.Debug, "system was unable to add postDto to DB.");
                return BadRequest(new { success = false });
            }
        }

        /// <summary>
        /// Used to set the isDeletedFlag on post object. Flag is being used to avoid hassles with EF self referencing table. 
        /// </summary>
        /// <param name="id">Id of the post to delete</param>
        /// <returns>Returns jsonresult with success value. </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            _logger.Log(LogLevel.Information,"attempted to execute - {0}, parameters - {1}", nameof(DeletePost), HttpContext.Request.Body);

            if (post == null)
            {
                _logger.Log(LogLevel.Debug, "Failed to execute {0} post id not found", nameof(DeletePost));
                return NotFound();
            }

            post.IsDeletedFlag = true;
            _unitOfWork.PostRepository.Update(post);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                _logger.Log(LogLevel.Information, "Post was deleted from DB successfully.");
                return Ok(new { success = true });
            }
            else
            {
                _logger.Log(LogLevel.Debug, "error happened trying to delete post from DB");
                return BadRequest(new { success = false });
            }
        }

        /// <summary>
        /// Used to set the isHiddenFromUserProfile on post object. Flag is being used to avoid hassles with EF self referencing table. 
        /// </summary>
        /// <param name="id">Id of the post to hide</param>
        /// <returns>IActionResult with success indicator whether post was successfully hidden or not. </returns>
        [HttpDelete("Hide/{id}")]
        public async Task<IActionResult> HidePost(int? id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            post.IsHiddenFromUserProfile = true;
            _unitOfWork.PostRepository.Update(post);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0 ? Ok(new {success = true}) : BadRequest(new {success = false});
        }



        /// <summary>
        /// Reactivates social media post. Changes the isDeletedFlag of object in db. 
        /// </summary>
        /// <param name="id">id of the post the developer wants reactivated. </param>
        /// <returns>IActionResult with success indicator whether post was successfully reactivated or not. </returns>
        [HttpDelete("Reactivate/{id}")]
        public async Task<IActionResult> ReactivatePost(int? id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            post.IsDeletedFlag = false;
            _unitOfWork.PostRepository.Update(post);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0 ? Ok(new {success=true}) : BadRequest(new {success=false});
        }

        private bool PostExists(int? id)
        {
            var post = _unitOfWork.PostRepository.Find(p => p.Id.Equals(id));
            return post.Any();
        }
    }
}
