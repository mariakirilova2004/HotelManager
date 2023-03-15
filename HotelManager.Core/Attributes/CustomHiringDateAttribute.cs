using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Attributes
{
    public class CustomHiringDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            value = (DateTime)value;
            // This assumes inclusivity
            if ((DateTime.Now.AddYears(-100).CompareTo(value) <= 0 && DateTime.Now.AddYears(10).CompareTo(value) >= 0) || value == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Enter a valid date.");
            }
        }
    }
}
