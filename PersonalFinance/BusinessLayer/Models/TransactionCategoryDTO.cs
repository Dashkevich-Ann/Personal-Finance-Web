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
    }

    public enum TransactionType
    {
        Income = 0,
        Cost = 1
    }
}
