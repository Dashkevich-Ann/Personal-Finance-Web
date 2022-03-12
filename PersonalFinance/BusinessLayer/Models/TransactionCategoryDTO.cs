using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class TransactionCategoryDTO
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal? MonthLimit { get; set; }
        public TransactionType Type { get; set; }
        public string LimitDisplayValue => Type == TransactionType.Income ? "-" : MonthLimit?.ToString("C") ?? "-";

        public override bool Equals(object obj)
        {
            var other = obj as TransactionCategoryDTO;

            return other != null
                   && other.UserId == UserId
                   && other.CategoryId == CategoryId
                   && other.Type == Type;

        }

        public override int GetHashCode()
        {
            return CategoryId + UserId + (int)Type + (int)(MonthLimit ?? 0);
        }
    }

    public enum TransactionType
    {
        Income = 0,
        Cost = 1
    }
}
