using FaceIDLoginProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FaceIDLoginProject.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } // Bảng User

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cho bảng Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique(); // Đảm bảo tên người dùng không bị trùng
        }
    }
}
