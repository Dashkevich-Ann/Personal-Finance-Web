using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Models
{
    public class AdminEditUserModel
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [BindProperty, DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(50)]
        public string Email { get; set; }

        [RequaredAnyItem(ErrorMessage ="Any role is required")]
        public RoleModel[] Roles { get; set; }
    }
}
