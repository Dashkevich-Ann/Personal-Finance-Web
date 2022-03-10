using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configuration
{
    internal class IncomeCategoryConfiguration : IEntityTypeConfiguration<IncomeCategory>
    {

        public void Configure(EntityTypeBuilder<IncomeCategory> builder)
        {
            builder.ToTable("IncomeCategory").HasKey(b => b.IncomeCategoryId);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(60);

        }
    }
}
