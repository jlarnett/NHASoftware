﻿namespace NHASoftware.Models.ForumModels
{
    public class ForumComment
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public DateTime CreationDate { get; set; }
        public ApplicationUser? User { get; set; }
        public string UserId { get; set; }
        public ForumPost? ForumPost { get; set; }
        public int ForumPostId { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
    }
}
