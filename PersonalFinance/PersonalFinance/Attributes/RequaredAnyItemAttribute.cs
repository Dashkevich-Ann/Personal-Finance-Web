using PersonalFinance.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Attributes
{
    public class RequaredAnyItemAttribute : ValidationAttribute
    {
        protected override ValidationResult
                IsValid(object value, ValidationContext validationContext)
        {
            var items = ((IEnumerable<object>)value).Cast<IChecked>();

            if (items.Any(x => x.IsChecked))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult
                    (string.IsNullOrEmpty(ErrorMessage) ? "Require any elements" : ErrorMessage);
        }
    }
}
