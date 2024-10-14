﻿using Microsoft.AspNetCore.Identity;
namespace api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; } 
    }
}
