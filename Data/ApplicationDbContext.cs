using Microsoft.EntityFrameworkCore;
using BlogWebApp.Models;
using BCrypt.Net;

namespace BlogWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Username = "a@a.a",
                    Password = "$2a$11$OGlqtho7OXl/dt1KWzZFReKnVyKbrYOtr3.cBCCQdVdsLG9iLL1j6",
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Admin",
                    IsApproved = true
                },
                new User
                {
                    Username = "c@c.c",
                    Password = "$2a$11$OGlqtho7OXl/dt1KWzZFReKnVyKbrYOtr3.cBCCQdVdsLG9iLL1j6",
                    FirstName = "Contributor",
                    LastName = "User",
                    Role = "Contributor",
                    IsApproved = false
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=App_Data/app.db");
            }
        }
    }
}
