using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Entities
{
    public class CostCategory
    {
        public int CostCategoryId { get; set; } //Id category of cost
        public string Name { get; set; } //Cost's name
        public decimal CostLimit { get; set; } //Month amount limit which is default

        public int UserId { get; set; } //User's ID who creates category of cost
        public User User { get; set; } 
        public virtual ICollection<Cost> Costs { get; set; } //Collection of costs which belongs to the category

    }
}
