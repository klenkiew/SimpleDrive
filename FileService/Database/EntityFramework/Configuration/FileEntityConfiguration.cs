﻿using FileService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Database.EntityFramework.Configuration
{
    public class FileEntityConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.HasKey(file => file.Id).HasName("FileId");
            
            builder.Property(file => file.FileName).IsRequired();
            builder.Property(file => file.DateCreated).IsRequired();
            builder.Property(file => file.DateModified).IsRequired();
            builder.Property(file => file.Size).IsRequired();
            
            builder
                .HasMany(file => file.SharedWith)
                .WithOne(fs => fs.File)
                .HasForeignKey("FileId")
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasIndex(file => file.FileName);
            
            builder.ToTable("Files");
        }
    }
}