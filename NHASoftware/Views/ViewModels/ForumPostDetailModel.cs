﻿using NHASoftware.Entities.Forums;

namespace NHASoftware.ViewModels
{
    public class ForumPostDetailModel
    {
        public ForumPost ForumPost { get; set; } = new ForumPost();
        public List<ForumComment> ForumComments { get; set; } = new List<ForumComment>();
    }
}
