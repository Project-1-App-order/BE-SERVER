using api.Validations;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace api.DTOs.Authentication
{
    public class RegisterDTO
    {
        private string email;
        [StringLength(40, MinimumLength = 16, ErrorMessage = "Length Email Invalid")]
        [Required(ErrorMessage = "Emtpy email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email invalid")]
        public required string Email
        {
            get => email;
            set => email = value.Trim(); 
        }
        [PasswordStrength]
        public required string Password { get; set; }
    }
}
