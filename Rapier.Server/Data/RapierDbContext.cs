using Microsoft.EntityFrameworkCore;

namespace Rapier.Server.Data
{
    public class RapierDbContext : DbContext
    {
        public RapierDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
