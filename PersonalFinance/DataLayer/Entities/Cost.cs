using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Entities
{
    public class Cost
    {
        public int CostId { get; set; } //ID of the cost transaction
        public DateTime Date { get; set; } //Date when the transaction is made
        public decimal Amount { get; set; } //Amount of the transaction
        public string Comment { get; set; }

        public int CostCategoryId { get; set; } //ID of the cost category
        public CostCategory CostCategory { get; set; } //Category of cost which belongs the transaction
        public int UserId { get; set; } //User's ID who makes the transaction
        public User User { get; set; }
    }
}
