using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Models
{
    public class RegisterModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(ErrorMessage = "No email specified")]
        public string Email { get; set; }

        [Required(ErrorMessage = "No login specified")]
        public string Login { get; set; }

        [Required(ErrorMessage = "No password specified")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password was entered incorrectly")]
        public string ConfirmPassword { get; set; }

        [BindProperty, DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
    }
}
