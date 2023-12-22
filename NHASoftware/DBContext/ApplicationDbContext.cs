using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Entities.Session;

namespace NHA.Website.Software.DBContext;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Post>().HasMany(p => p.Comments).WithOne(p => p.ParentPost).HasForeignKey(p => p.ParentPostId);
        builder.Entity<Post>().HasMany(p => p.PostImages).WithOne(p => p.Post).OnDelete(DeleteBehavior.Restrict);
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
    public DbSet<SessionHistoryEvent>? SessionHistory { get; set; }

    public void DeleteMyEntity(Post post)
    {
        var target = Posts!.Include(p => p.Comments).FirstOrDefault(p => p.Id == post.Id);
    }

    private void RecursiveDelete(Post parent)
    {
        if (parent.Comments != null)
        {
            var children = Posts!.Include(p => p.Comments).Where(p => p.ParentPostId == parent.Id);

            foreach (var child in children)
            {
                RecursiveDelete(child);
            }
        }

        Posts!.Remove(parent);
    }

}
