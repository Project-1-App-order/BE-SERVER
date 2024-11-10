using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Authentication
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Emtpy email")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Emtpy Password")]
        public required string Password { get; set; }
    }
}
