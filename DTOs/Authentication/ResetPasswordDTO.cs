using api.Validations;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Authentication
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Emtpy email")]
        public string email {  get; set; }
        public required string Email
        {
            get => email;
            set => email = value.Trim();
        }

        [Required(ErrorMessage = "Emtpy otp")]
        public required string Otp { get; set; }
        [PasswordStrength]
        public required string NewPassword { get; set; }
    }
}
