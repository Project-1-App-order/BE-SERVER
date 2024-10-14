using api.Validations;

namespace api.DTOs.Authentication
{
    public class ChangePasswordDTO
    {
        [PasswordStrength]
        public required string CurrentPassword { get; set; }
        [PasswordStrength]
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
