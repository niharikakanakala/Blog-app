using Microsoft.EntityFrameworkCore;
using Blog.WebAPI.Data;
using Blog.WebAPI.Models;

namespace Blog.WebAPI.Data
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration for entities

            base.OnModelCreating(modelBuilder);
        }
    }
}
