using api.Validations;

namespace api.DTOs.Authentication
{
    public class ResetPasswordDTO
    {
        public required string Email { get; set; }
        public required string Otp { get; set; }
        [PasswordStrength]
        public required string NewPassword { get; set; }
    }
}
