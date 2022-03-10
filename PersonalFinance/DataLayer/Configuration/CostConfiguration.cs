using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configuration
{
    internal class CostConfiguration : IEntityTypeConfiguration<Cost>
    {

        public void Configure(EntityTypeBuilder<Cost> builder)
        {
            builder.ToTable("Cost").HasKey(b => b.CostId);

            builder.Property(b => b.Date).IsRequired();
            builder.Property(b => b.Amount).IsRequired();
            builder.Property(b => b.Comment).HasMaxLength(200);

            builder.HasOne(c => c.CostCategory).WithMany(c => c.Costs).HasForeignKey(c => c.CostCategoryId);
        }
    }
}
