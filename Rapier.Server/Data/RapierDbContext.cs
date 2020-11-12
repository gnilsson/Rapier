using Microsoft.EntityFrameworkCore;
using System;

namespace Rapier.Server.Data
{
    public class RapierDbContext : DbContext
    {
        public RapierDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
                .HasMany(x => x.Blogs)
                .WithMany(x => x.Authors);
                //.UsingEntity<AuthorBlogs>(
                // x => x.HasOne(xs => xs.Blog).WithMany(),
                // x => x.HasOne(xs => xs.Author).WithMany())
                // .HasKey(x => new { x.AuthorId, x.BlogId });
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
