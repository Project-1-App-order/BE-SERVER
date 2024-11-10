using api.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace api.Validations
{
    public class PasswordStrengthAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Empty Password");
            }

            // Kiểm tra các điều kiệnx
            if (password.Length < 8 || password.Length > 16 ||
                !Regex.IsMatch(password, @"[A-Z]") ||    
                !Regex.IsMatch(password, @"[a-z]") ||   
                !Regex.IsMatch(password, @"\d") ||      
                !Regex.IsMatch(password, @"[\W_]"))      
            {
                return new ValidationResult( "invalid format" ); ;
            }

            return ValidationResult.Success;
        }
    }
}
