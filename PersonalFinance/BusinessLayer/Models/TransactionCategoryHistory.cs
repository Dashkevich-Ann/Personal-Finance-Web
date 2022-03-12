using System;
using System.Collections.Generic;

namespace BusinessLayer.Models
{
    public class TransactionCategoryHistory : TransactionCategoryDTO
    {
        public TransactionCategoryHistory(TransactionCategoryDTO dto)
        {
            UserId = dto.UserId;
            CategoryId = dto.CategoryId;
            Name = dto.Name;
            MonthLimit = dto.MonthLimit;
            Type = dto.Type;
        }

        public IEnumerable<CategoryMonth> CategoryHistory { get; set; }
    }

    public class CategoryMonth
    {
        public DateTime MonthYear { get; set; }

        public decimal Amount { get; set; }

        public decimal? MonthLimit { get; set; }

        public bool IsOverSpend => MonthLimit.HasValue && Amount > MonthLimit.Value;
    }
}