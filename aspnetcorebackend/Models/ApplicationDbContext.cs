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
    }
}