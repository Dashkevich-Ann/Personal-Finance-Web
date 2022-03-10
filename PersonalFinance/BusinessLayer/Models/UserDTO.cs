using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } //User's first name
        public string LastName { get; set; } //User's last name
        public string Email { get; set; }
        public string Login { get; set; } //User's login
        public string Password { get; set; } //User's account password
        public DateTime? DateOfBirth { get; set; } //User's date of birth
        public string Name => string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName)
            ? Login : $"{FirstName} {LastName}";

        public IEnumerable<Role> UserRoles { get; set; } //User's collections of roles
    }
}
