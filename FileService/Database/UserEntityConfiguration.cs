using FileService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Database
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id).HasName("Id");
            builder.Property(user => user.UserName).IsRequired();
            
            builder.HasIndex(user => user.UserName);

            builder
                .HasMany(user => user.OwnedFiles)
                .WithOne(file => file.Owner)
                .HasForeignKey(file => file.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.ToTable("Users");
        }
    }
}