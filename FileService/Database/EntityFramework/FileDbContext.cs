using FileService.Database.EntityFramework.Configuration;
using FileService.Model;
using Microsoft.EntityFrameworkCore;

namespace FileService.Database.EntityFramework
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FileEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FileShareEntityConfiguration());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
    }
}