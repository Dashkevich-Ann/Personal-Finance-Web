using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Configuration
{
    internal class CostCategoryConfiguration : IEntityTypeConfiguration<CostCategory>
    {
        public void Configure(EntityTypeBuilder<CostCategory> builder)
        {
            builder.ToTable("CostCategory").HasKey(b => b.CostCategoryId);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(60);
        }
    }
}
