using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Entities
{
    public class Income
    {
        public int IncomeId { get; set; } //ID of the income transaction
        public DateTime Date { get; set; } //Date when the transaction is made
        public decimal Amount { get; set; } //Amount of the transaction
        public string Comment { get; set; }

        public int IncomeCategoryId { get; set; } //ID of the income category
        public IncomeCategory IncomeCategory { get; set; } //Category of income which belongs the transaction
        public int UserId { get; set; } //User's ID who makes the transaction
        public User User { get; set; }
    }
}
