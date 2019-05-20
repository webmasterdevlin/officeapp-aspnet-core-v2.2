using aspnetcorebackend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace aspnetcorebackend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(d => d.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false).IsRequired();

                entity.Property(d => d.Description)
                    .HasMaxLength(140)
                    .IsUnicode(false).IsRequired();

                entity.Property(d => d.Head)
                    .HasMaxLength(50)
                    .IsUnicode(false).IsRequired();

                entity.Property(d => d.Code)
                    .HasMaxLength(6)
                    .IsUnicode(false).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Email)
                    .IsRequired();

                entity.Property(u => u.Password)
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}