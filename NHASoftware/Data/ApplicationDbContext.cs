using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Models;

namespace NHASoftware.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region OldManyToManyMapping

                //builder.Entity<BookAuthor>().HasKey(sc => new { sc.AuthorId, sc.BookId });
                //builder.Entity<BookAuthor>().HasOne<Author>(ab => ab.Author).WithMany(a => a.BookAuthors)
                //    .HasForeignKey(ab => ab.AuthorId);
                //builder.Entity<BookAuthor>().HasOne<Book>(ab => ab.Book).WithMany(b => b.BookAuthors).HasForeignKey(ab => ab.BookId);

            #endregion

        }

        //public DbSet<Book> Books { get; set; }
        //public DbSet<Author> Authors { get; set; }
        //public DbSet<BookAuthor> BookAuthors { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<TaskFrequency>  Frequencies { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }


    }
}