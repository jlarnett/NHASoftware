﻿using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;

namespace NHA.Website.Software.Services.Forums
{
    public class ForumCommentRepository : GenericRepository<ForumComment>, IForumCommentRepository
    {
        public ForumCommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ForumComment?> GetForumCommentWithLazyLoadingAsync(int? forumCommentId)
        {
            return await _context.ForumComments!
                .Include(f => f.ForumPost)
                .FirstOrDefaultAsync(m => m.Id == forumCommentId);
        }

        public async Task<List<ForumComment>> GetForumPostCommentsAsync(int? forumPostId)
        {
            return await _context.ForumComments!.Where(c => c.ForumPostId == forumPostId).Include(p => p.User).ToListAsync();
        }

        public async Task<int> GetNumberOfCommentsForPost(int? forumPostId)
        {
            return await _context.ForumComments!.CountAsync(c => c.ForumPostId == forumPostId);
        }
    }
}
