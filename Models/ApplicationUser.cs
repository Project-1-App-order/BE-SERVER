using Microsoft.AspNetCore.Identity;
namespace api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }
}
