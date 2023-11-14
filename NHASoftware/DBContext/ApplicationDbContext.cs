﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NHASoftware.Entities;
using NHASoftware.Entities.Anime;
using NHASoftware.Entities.Forums;
using NHASoftware.Entities.Identity;
using NHASoftware.Entities.FriendSystem;
using NHASoftware.Entities.Social_Entities;
using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Entities.Social_Entities;

namespace NHASoftware.DBContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<ForumSection>? ForumSections { get; set; }
        public DbSet<ForumTopic>? ForumTopics { get; set; }
        public DbSet<ForumPost>? ForumPosts { get; set; }
        public DbSet<ForumComment>? ForumComments { get; set; }
        public DbSet<AnimePage>? AnimePages { get; set; }
        public DbSet<AnimeEpisode>? AnimeEpisodes { get; set; }
        public DbSet<Post>? Posts { get; set; }
        public DbSet<UserLikes>? UserLikes { get; set; }
        public DbSet<FriendRequest>? FriendRequests { get; set; }
        public DbSet<Friends>? Friends { get; set; }
        public DbSet<PostImage>? PostImages { get; set; }

    }
}