using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace api.Validations
{
    public class PasswordStrengthAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required");
            }

            // Danh sách để chứa tất cả các thông báo lỗi
            var errors = new List<string>();

            if (password.Length < 8 || password.Length > 16)
            {
                return new ValidationResult("Password must be between 8 and 16 characters");
            }

            // Kiểm tra có ít nhất một chữ cái viết hoa
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                errors.Add("Password must contain at least one uppercase letter");
            }

            // Kiểm tra có ít nhất một chữ cái viết thường
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                errors.Add("Password must contain at least one lowercase letter");
            }

            // Kiểm tra có ít nhất một số
            if (!Regex.IsMatch(password, @"\d"))
            {
                errors.Add("Password must contain at least one digit");
            }

            // Kiểm tra có ít nhất một ký tự đặc biệt
            if (!Regex.IsMatch(password, @"[\W_]"))
            {
                errors.Add("Password must contain at least one special character");
            }

            // Nếu có lỗi, trả về tất cả các lỗi
            if (errors.Count > 0)
            {
                return new ValidationResult(string.Join(", ", errors));
            }

            return ValidationResult.Success;
        }
    }
}
