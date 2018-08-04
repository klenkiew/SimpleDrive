using FileService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Database.EntityFramework.Configuration
{
    public class FileShareEntityConfiguration : IEntityTypeConfiguration<FileShare>
    {
        public void Configure(EntityTypeBuilder<FileShare> builder)
        {
            builder.Property("FileId");
            builder.Property("UserId");
            builder.HasKey("FileId", "UserId");
        }
    }
}