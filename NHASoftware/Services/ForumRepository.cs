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

        public async Task<List<ForumPost>> GetForumPostsAsync()
        {
            return await _context.ForumPosts
                .Include(f => f.User)
                .Include(f => f.ForumTopic)
                .ToListAsync();
        }

        public async Task<List<ForumComment>> GetForumCommentsAsync()
        {
            return await _context.ForumComments
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<List<ForumSection>> GetForumSectionsAsync()
        {
            return await _context.ForumSections.ToListAsync();
        }
        public async Task<List<ForumTopic>> GetForumTopicsAsync()
        {
            return await _context.ForumTopics.ToListAsync();
        }

        public async Task<ForumPost> GetForumPostDetails(int? forumPostId)
        {
            var post = await _context.ForumPosts
                    .Include(f => f.ForumTopic)
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(m => m.Id == forumPostId);

            return post;
        }


        public async Task<ForumTopic> GetForumTopic(int? forumTopicId)
        {
            var forumTopic = await _context.ForumTopics
                .Include(f => f.ForumSection)
                .FirstOrDefaultAsync(m => m.Id == forumTopicId);

            return forumTopic;
        }


        public async Task<List<ForumPost>> GetForumTopicPosts(int? forumTopicId)
        {
            return await _context.ForumPosts.Where(c => c.ForumTopicId == forumTopicId).ToListAsync();
        }
    }
}
