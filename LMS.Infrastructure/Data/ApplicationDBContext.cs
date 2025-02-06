using LMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
       /* public ApplicationDBContext()
        {
            
        }*/
        //composite key
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<BookGenre>().HasKey(x => new { x.BookId, x.GenreId });//composite key
        }
       public DbSet<Book> Books { get; set; }
       public DbSet<Borrow> Borrows { get; set; }
       public DbSet<Genre> Genres { get; set; }
       public DbSet<BookGenre> BookGenres { get; set; }



    }
}
