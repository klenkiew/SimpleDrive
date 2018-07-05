using AuthenticationService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Database
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(user => user.Id).HasName("Id");
            modelBuilder.Entity<User>().Property(user => user.UserName).IsRequired();
            
            modelBuilder.Entity<User>().HasIndex(user => user.UserName);
            modelBuilder.Entity<User>().HasIndex(user => user.Email);

            modelBuilder.Entity<User>().ToTable("Users");
            
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
    }
}