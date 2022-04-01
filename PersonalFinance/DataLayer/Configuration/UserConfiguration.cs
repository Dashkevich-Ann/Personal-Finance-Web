using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configuration
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User").HasKey(b => b.UserId);
            builder.Property(b => b.FirstName).HasMaxLength(60);
            builder.Property(b => b.LastName).HasMaxLength(100);
            builder.Property(b => b.Email).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Login).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Password).IsRequired().HasMaxLength(100);

            builder.HasMany(b => b.UserRoles).WithOne(u => u.User).HasForeignKey(u => u.UserId);
            builder.HasMany(b => b.Incomes).WithOne(i => i.User).HasForeignKey(i => i.UserId);
            builder.HasMany(b => b.Costs).WithOne(c => c.User).HasForeignKey(c => c.UserId);
            builder.HasMany(b => b.IncomeCategories).WithOne(i => i.User).HasForeignKey(i => i.UserId);
            builder.HasMany(b => b.CostCategories).WithOne(c => c.User).HasForeignKey(c => c.UserId);
        }
    }
}
