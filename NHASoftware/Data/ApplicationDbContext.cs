using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Models;

namespace NHASoftware.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AuthorBook>().HasKey(sc => new { sc.AuthorId, sc.BookId });

            builder.Entity<AuthorBook>().HasOne<Author>(ab => ab.Author).WithMany(a => a.AuthorBooks)
                .HasForeignKey(ab => ab.AuthorId);

            builder.Entity<AuthorBook>().HasOne<Book>(ab => ab.Book).WithMany(b => b.AuthorBooks)
                .HasForeignKey(ab => ab.BookId);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorBook> AuthorBooks { get; set; }
        
    }
}