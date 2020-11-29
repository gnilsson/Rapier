using Microsoft.EntityFrameworkCore;

namespace Rapier.Server.Data
{
    public class RapierDbContext : DbContext
    {
        public RapierDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
