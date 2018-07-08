﻿using FileService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Database
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id).HasName("Id");
            builder.Property(user => user.Username).IsRequired();
            
            builder.HasIndex(user => user.Username);

            builder
                .HasMany(user => user.OwnedFiles)
                .WithOne(file => file.Owner)
                .HasForeignKey(file => file.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(user => user.SharedFiles)
                .WithOne(fs => fs.User)
                .HasForeignKey(fs => fs.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.ToTable("Users");
        }
    }
}