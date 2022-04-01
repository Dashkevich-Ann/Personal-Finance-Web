using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Models
{
    public class PasswordRestoreModel
    {
        [Required]
        public string Email { get; set; }
    }
}