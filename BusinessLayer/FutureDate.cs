using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class FutureDate : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true;

            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.Now;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be a future date and time!";
        }
    }
}
