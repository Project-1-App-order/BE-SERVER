﻿using Microsoft.AspNetCore.Authentication;
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
using static api.Responses.ServiceResponse;

namespace api.Services.Functions
{
    public class AuthenticationService : api.Services.Interfaces.IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthenticationService(ApplicationDbContext context,
                                     UserManager<ApplicationUser> userManager,
                                     IConfiguration configuration,
                                     ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _tokenService = tokenService;
        }
        public async Task<GeneralResponse> ResgisterAsync(RegisterDTO registerDTO)
        {
            
            if (registerDTO is null)
            {
                return new GeneralResponse(false, "Request is null", null);
            }
            var email = registerDTO.Email.Trim();
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null) { return new GeneralResponse(false, "Email already exists", null); }
            var newUser = new ApplicationUser
            {
                Email = email,
                UserName = email,
                Id = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(newUser, registerDTO.Password.Trim());
            if (result.Succeeded)
            {
                return new GeneralResponse(true, "Register succesfully", newUser.Id);
            }
            return new GeneralResponse(false, "Resgister failed", null);
            

        }
        public JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSiginKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMonths(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSiginKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }
        public async Task<LoginResponse> LoginAsync(LoginDTO request)
        {
            if (request is null) return new LoginResponse(false, "Request is null", null!);
            var email = request.Email.Trim();
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (existingUser == null) return new LoginResponse(false, "User doesn't exist", null!);
            
            
            var currentTokens = await _context.UserTokens.FirstOrDefaultAsync(x => x.UserId == existingUser.Id);
            if (currentTokens != null)
            {
                await _tokenService.RevokeTokenAsync(currentTokens.Value, DateTime.UtcNow.AddMinutes(2));
            }
            
            var isPasswordValid = await _userManager.CheckPasswordAsync(existingUser, request.Password);
            if (!isPasswordValid) return new LoginResponse(false, "Invalid password", null!);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, existingUser.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var jwtToken = GetToken(authClaims);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var tokenDescriptor = new IdentityUserToken<string>
            {
                UserId = existingUser.Id,
                LoginProvider = "JWT",
                Name = "AccessToken",
                Value = tokenString
            };

            await _userManager.SetAuthenticationTokenAsync(existingUser, tokenDescriptor.LoginProvider, tokenDescriptor.Name, tokenString);

            return new LoginResponse(true, "Logined successfully", tokenString);
        }

        public string GenerateRandomOTP()
        {
            Random generator = new();
            string otp = generator.Next(0, 999999).ToString("D6");
            return otp;
        }

        public async Task RemoveExpiredOtps()
        {
            var currentTime = DateTime.UtcNow;
            await _context.OtpStorages
                .Where(o => o.ExpiryTime < currentTime)
                .ExecuteDeleteAsync();
        }

    }
}
