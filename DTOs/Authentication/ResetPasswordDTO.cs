using api.Validations;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Authentication
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Emtpy email")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Emtpy otp")]
        public required string Otp { get; set; }
        [Required(ErrorMessage = "Emtpy Password")]
        [PasswordStrength]
        public required string NewPassword { get; set; }
    }
}
