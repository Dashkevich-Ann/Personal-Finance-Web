using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configuration
{
    internal class IncomeConfiguration : IEntityTypeConfiguration<Income>
    {

        public void Configure(EntityTypeBuilder<Income> builder)
        {
            builder.ToTable("Income").HasKey(b => b.IncomeId);
            builder.Property(b => b.Date).IsRequired();
            builder.Property(b => b.Amount).IsRequired();
            builder.Property(b => b.Comment).HasMaxLength(200);

            builder.HasOne(i => i.IncomeCategory).WithMany(i => i.Incomes).HasForeignKey(i => i.IncomeCategoryId);

        }
    }
}
