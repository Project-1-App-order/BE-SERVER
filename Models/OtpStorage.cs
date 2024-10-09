using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class OtpStorage
    {
        [Key]
        public string? Id { get; set; }
        [ForeignKey(nameof(Id))]
        public string? UserId { get; set; }
        public string? Otp { get; set; }
        public DateTime ExpiryTime { get; set; }
        public ApplicationUser? applicationUser { get; set; }
    }
}
