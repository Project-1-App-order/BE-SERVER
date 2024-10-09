using Microsoft.AspNetCore.Authentication;
using api.Services.Interfaces;
using api.Data;
using Microsoft.AspNetCore.Identity;
using api.DTOs.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Org.BouncyCastle.Asn1.Ocsp;
using MySqlX.XDevAPI.Common;
using Microsoft.EntityFrameworkCore;
using api.Models;
using Newtonsoft.Json.Linq;
using api.Responses;

namespace api.Services.Functions
{
    public class AuthenticationService : api.Services.Interfaces.IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        public async Task<ServiceResponse.GeneralResponse> ResgisterAsync(RegisterDTO registerDTO)
        {
            var user = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (user != null) { return new ServiceResponse.GeneralResponse(false, "Email already exists", null); }
            if (registerDTO is null) return new ServiceResponse.GeneralResponse(false, "Request is null", null);
            var newUser = new ApplicationUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                Id = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(newUser, registerDTO.Password);
            if (result.Succeeded)
            {
                return new ServiceResponse.GeneralResponse(true, "Register is sucessfully", null);
            }
            else
            {
                return new ServiceResponse.GeneralResponse(false, "Register failed", null);
            }

        }







    }
}
