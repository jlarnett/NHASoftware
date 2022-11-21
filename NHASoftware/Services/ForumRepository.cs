using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NHASoftware.Data;
using NHASoftware.Models;
using NHASoftware.Models.ForumModels;

namespace NHASoftware.Services
{
    public class ForumRepository : IForumRepository
    {
        private readonly ApplicationDbContext _context;

        public ForumRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns ALL forum posts in DB. 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ForumPost>> GetForumPostsAsync()
        {
            return await _context.ForumPosts
                .Include(f => f.User)
                .Include(f => f.ForumTopic)
                .ToListAsync();
        }

        /// <summary>
        /// Returns ALL forum comments in DB.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ForumComment>> GetForumCommentsAsync()
        {
            return await _context.ForumComments
                .Include(c => c.User)
                .ToListAsync();
        }

        /// <summary>
        /// Returns ALL forum sections in DB. 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ForumSection>> GetForumSectionsAsync()
        {
            return await _context.ForumSections.ToListAsync();
        }

        /// <summary>
        /// Returns ALL forum topics in DB. 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ForumTopic>> GetForumTopicsAsync()
        {
            return await _context.ForumTopics.ToListAsync();
        }

        /// <summary>
        /// Returns forum post for supplied Id. If Id is not found Null value is returned.
        /// </summary>
        /// <param name="forumPostId">Id for the forum post the user is trying to get from DB. </param>
        /// <returns></returns>
        public async Task<ForumPost> GetForumPostAsync(int? forumPostId)
        {
            return await _context.ForumPosts
                    .Include(f => f.ForumTopic)
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.Id == forumPostId);

        }

        /// <summary>
        /// Returns the Forum Section for supplied forumSectionId. If Id is not found Null value is returned.
        /// </summary>
        /// <param name="forumSectionId">Id for the forum section the user is trying to get from DB. </param>
        /// <returns></returns>
        public async Task<ForumSection> GetForumSectionAsync(int? forumSectionId)
        {
            return await _context.ForumSections
                .FirstOrDefaultAsync(m => m.Id == forumSectionId);
        }

        /// <summary>
        /// Returns the forum topic for supplied forumTopicId. If Id is not found Null value is returned.
        /// </summary>
        /// <param name="forumTopicId">forumTopicId the user is trying to retrieve from DB. </param>
        /// <returns></returns>
        public async Task<ForumTopic> GetForumTopicAsync(int? forumTopicId)
        {
            return await _context.ForumTopics
                .Include(f => f.ForumSection)
                .FirstOrDefaultAsync(m => m.Id == forumTopicId);
        }

        /// <summary>
        /// Returns list of ALL forum post for a specific forum topic. 
        /// </summary>
        /// <param name="forumTopicId">forumTopicId the user wants to retrieve ALL forum posts for. </param>
        /// <returns></returns>
        public async Task<List<ForumPost>> GetForumTopicPostsAsync(int? forumTopicId)
        {
            return await _context.ForumPosts.Where(c => c.ForumTopicId == forumTopicId).ToListAsync();
        }


        /// <summary>
        /// Returns ALL comments for forum post. 
        /// </summary>
        /// <param name="forumPostId">forumPostId the user wants comments for. </param>
        /// <returns></returns>
        public async Task<List<ForumComment>> GetForumPostCommentsAsync(int? forumPostId)
        {
            return await _context.ForumComments.Where(c => c.ForumPostId == forumPostId).Include(p => p.User).ToListAsync();
        }


    }
}
