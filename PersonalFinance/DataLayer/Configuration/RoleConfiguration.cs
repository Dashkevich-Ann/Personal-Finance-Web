using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DataLayer.Configuration
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role").HasKey(b => b.RoleId);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(50);

            builder.HasMany(b => b.UserRoles).WithOne(r => r.Role).HasForeignKey(r => r.RoleId);
        }
    }
}
