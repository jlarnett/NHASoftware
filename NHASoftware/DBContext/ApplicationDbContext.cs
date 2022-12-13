using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Entities;
using NHASoftware.Entities.Anime;
using NHASoftware.Entities.Forums;
using NHASoftware.Entities.Identity;
using NHASoftware.ConsumableEntities.DTOs;
using NHASoftware.Entities.Social_Entities;

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

            builder.Entity<Subscription>().HasOne(e => e.TaskItem).WithMany(e => e.Subscriptions)
                .OnDelete(DeleteBehavior.SetNull);

            #region OldManyToManyMapping

            //builder.Entity<BookAuthor>().HasKey(sc => new { sc.AuthorId, sc.BookId });
            //builder.Entity<BookAuthor>().HasOne<Author>(ab => ab.Author).WithMany(a => a.BookAuthors)
            //    .HasForeignKey(ab => ab.AuthorId);
            //builder.Entity<BookAuthor>().HasOne<Book>(ab => ab.Book).WithMany(b => b.BookAuthors).HasForeignKey(ab => ab.BookId);

            #endregion

        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<TaskFrequency>  Frequencies { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<ForumSection> ForumSections { get; set; }
        public DbSet<ForumTopic> ForumTopics { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<ForumComment> ForumComments { get; set; }
        public DbSet<AnimePage> AnimePages { get; set; }
        public DbSet<AnimeEpisode> AnimeEpisodes { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}