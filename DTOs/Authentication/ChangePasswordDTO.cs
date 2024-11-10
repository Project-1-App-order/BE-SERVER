using api.Validations;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Authentication
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Emtpy Password")]
        public required string CurrentPassword { get; set; }
        [PasswordStrength]
        [Required(ErrorMessage = "Emtpy Password")]
        public required string NewPassword { get; set; }
        [Required(ErrorMessage = "Emtpy Password")]
        public required string ConfirmPassword { get; set; }
    }
}
