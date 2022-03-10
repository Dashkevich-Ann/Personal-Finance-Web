
using System;
using System.Collections.Generic;


namespace DataLayer.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } //User's first name
        public string LastName { get; set; } //User's last name
        public string Email { get; set; }
        public string Login { get; set; } //User's login
        public string Password { get; set; } //User's account password
        public DateTime? DateOfBirth { get; set; } //User's date of birth

        public virtual ICollection<CostCategory> CostCategories { get; set; } //User's categories of cost
        public virtual ICollection<IncomeCategory> IncomeCategories { get; set; } //User's categories of income
        public virtual ICollection<Cost> Costs { get; set; } //User's transactions of cost
        public virtual ICollection<Income> Incomes { get; set; } //User's transactions of income
        public virtual ICollection<UserRole> UserRoles { get; set; } //User's collections of roles
    }
}
