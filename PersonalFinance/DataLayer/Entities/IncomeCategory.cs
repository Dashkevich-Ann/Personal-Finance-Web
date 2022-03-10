using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Entities
{
    public class IncomeCategory
    {
        public int IncomeCategoryId { get; set; } ////Id category of income
        public string Name { get; set; } //Income's name

        public int UserId { get; set; } //User's ID who creates category of income
        public User User { get; set; } 
        public virtual ICollection<Income> Incomes { get; set; } //Collection of incomes which belongs to the category
    }
}
