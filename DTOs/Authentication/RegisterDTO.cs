using api.Validations;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace api.DTOs.Authentication
{
    public class RegisterDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email invalid")]
        public required string Email { get; set; }
        [PasswordStrength]
        public required string Password { get; set; }
    }
}
