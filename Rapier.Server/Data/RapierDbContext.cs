using Microsoft.EntityFrameworkCore;
using Rapier.External;
using System;
using System.Collections.Generic;

namespace Rapier.Server.Data
{
    public class RapierDbContext : DbContext
    {
        public RapierDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //      modelBuilder.Entity<IEntity>().Metadata.SetPropertyAccessMode(x )
            //modelBuilder.Entity<Blog>()
            //    .HasMany(x => x.Authors)
            //    .WithMany(x => x.Blogs);
            //modelBuilder.Entity<Author>()
            //    .HasMany(x => x.Blogs)
            //    .WithMany(x => x.Authors);
            //.UsingEntity<AuthorBlogs>(
            // x => x.HasOne(xs => xs.Blog).WithMany(),
            // x => x.HasOne(xs => xs.Author).WithMany())
            // .HasKey(x => new { x.AuthorId, x.BlogId });
            //    Authors.SingleAsync(x => x.Blogs.Add())
            //var f = new Author()
            //{
            //    Profession = Enums.ProfessionCategory.Programmer,
            //    Blogs = new List<Blog> { new Blog { p } }
            //};
            //var a = new List<Blog>();
            //a.AddRange(new[] { new Blog() });
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
