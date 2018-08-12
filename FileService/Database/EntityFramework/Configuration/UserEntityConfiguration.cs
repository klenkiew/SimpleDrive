using FileService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Database.EntityFramework.Configuration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id).HasName("UserId");
            builder.Property(user => user.Username).IsRequired();
            
            builder.HasIndex(user => user.Username);

            builder
                .HasMany(user => user.OwnedFiles)
                .WithOne(file => file.Owner);

            builder
                .HasMany(user => user.SharedFiles)
                .WithOne(fs => fs.User)
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.ToTable("Users");
        }
    }
}