namespace api.DTOs.Authentication
{
    public class ResetPasswordDTO
    {
        public string email { get; set; }
        public string otp { get; set; }
        public string newPassword { get; set; }
    }
}
