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

        // ✅ 기존 User 테이블과 함께 Article 테이블도 추가
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Admin & Contributor 기본 계정 시드 데이터 추가
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Username = "a@a.a",
                    Password = "$2a$11$OGlqtho7OXl/dt1KWzZFReKnVyKbrYOtr3.cBCCQdVdsLG9iLL1j6", // 해시된 P@$$w0rd
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Admin",
                    IsApproved = true
                },
                new User
                {
                    Username = "c@c.c",
                    Password = "$2a$11$OGlqtho7OXl/dt1KWzZFReKnVyKbrYOtr3.cBCCQdVdsLG9iLL1j6", // 해시된 P@$$w0rd
                    FirstName = "Contributor",
                    LastName = "User",
                    Role = "Contributor",
                    IsApproved = false // 승인 필요
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
