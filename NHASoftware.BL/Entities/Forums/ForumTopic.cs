﻿namespace NHASoftware.BL.Entities.Forums
{
    public class ForumTopic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ThreadCount { get; set; }
        public int PostCount { get; set; }
        public DateTime LastestPost { get; set; }
        public ForumSection? ForumSection { get; set; }
        public int ForumSectionId { get; set; }
    }
}
