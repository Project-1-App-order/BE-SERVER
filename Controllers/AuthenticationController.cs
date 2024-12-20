﻿using api.Data;
using api.DTOs.Authentication;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Responses;
using Microsoft.AspNetCore.Identity;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using api.Services.MailServices;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _mailService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        public AuthenticationController(IAuthenticationService authenticationService, 
                                        ApplicationDbContext context, 
                                        UserManager<ApplicationUser> userManager, 
                                        IMailService mailService,
                                        SignInManager<ApplicationUser> signInManager,
                                        ITokenService tokenService)
        {
            _authenticationService = authenticationService;
            _context = context;
            _userManager = userManager;
            _mailService = mailService;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            if( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.ResgisterAsync(registerDto);
            if (result.Flag == false)
            {
               return StatusCode(StatusCodes.Status400BadRequest,
                                  new Response { Status = result.Flag.ToString(), StatusMessage = result.Message });

            }

            return Ok(new{result.userId});
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authenticationService.LoginAsync(loginDto);
            if (result.Flag == false)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", StatusMessage = result.Message });
            }
            return Ok(result.Token);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {

            var currentToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var userToken = _context.UserTokens.FirstOrDefault(ut => ut.Value == currentToken);
            if (userToken != null)
            {
                _context.UserTokens.Remove(userToken);
                await _signInManager.SignOutAsync();
                await _context.SaveChangesAsync();
                await _tokenService.RevokeTokenAsync(currentToken, DateTime.UtcNow.AddMinutes(2));
                return StatusCode(StatusCodes.Status200OK, new { message = "Logged out successfully from current device" });

            }
            return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", StatusMessage = "Logout failed"});
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { Status = "Error", StatusMessage = "User not found" });
            }
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == userName);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { Status = "Error", StatusMessage = "User not found" });
            }

            var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, changPasswordModel.CurrentPassword);
            if (!isOldPasswordValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", StatusMessage = "Old password is incorrect" });
            }

            var result = await _userManager.ChangePasswordAsync(user, changPasswordModel.CurrentPassword, changPasswordModel.NewPassword);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", StatusMessage = "failed" });
            }

            return StatusCode(StatusCodes.Status200OK, new { Status = "Sucessed", StatusMessage = "Change Password sucessfully" });
        }

        [HttpPost]
        public async Task<IActionResult> SendOTP(string Email)
        {
            var email = Email.Trim();
            if (string.IsNullOrEmpty(email)) {
                   return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", StatusMessage = "empty email" });

            }
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                var existingOtp = await _context.OtpStorages.FirstOrDefaultAsync(o => o.UserId == existingUser.Id);
                if (existingOtp != null)
                {
                    _context.OtpStorages.Remove(existingOtp);
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", StatusMessage = "User not found" });
            }

            string otp = _authenticationService.GenerateRandomOTP();
            var otpRecord = new OtpStorage
            {
                Id = Guid.NewGuid().ToString(),
                UserId = existingUser.Id,
                Otp = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(2)
            };

            _context.OtpStorages.Add(otpRecord);
            await _context.SaveChangesAsync();

            string body = $@"Your OTP is: {otp}. Note: this OTP will be out of time after 2 minutes";

            var message = new Messages(new string[] { email }, "OTP Request", body);
            _mailService.SendEmail(message);
            return Ok($"OTP sent to email: {otp}");
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string userEmail, string otp)
        {
            var email = userEmail.Trim();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { Status = "Error", StatusMessage = "User not found, email does not exist" });
            }
            var exignOtp = await _context.OtpStorages.FirstOrDefaultAsync(o => o.Otp == otp && o.UserId == user.Id);
            if (exignOtp == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", StatusMessage = "otp invalid" });
            }
            if (exignOtp.ExpiryTime <= DateTime.UtcNow)
            {
                return StatusCode(StatusCodes.Status410Gone, new { Status = "Error", StatusMessage = "OTP is expired" });
            }
            return StatusCode(StatusCodes.Status200OK, new { Status = "Success", StatusMessage = "OTP is valid" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var email = resetmodel.Email.Trim();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Status = "Error", StatusMessage = "user not found" });

            await _authenticationService.RemoveExpiredOtps();

            var otpRecord = await _context.OtpStorages.FirstOrDefaultAsync(o => o.UserId == user.Id && o.Otp == resetmodel.Otp);
            if (otpRecord == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Status = "Error", StatusMessage = "OTP not found" });
            if (otpRecord.ExpiryTime <= DateTime.UtcNow) return StatusCode(StatusCodes.Status410Gone, new { Status = "Error", StatusMessage = "otp expired" });


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetmodel.NewPassword);

            if (result.Succeeded)
            {
                _context.OtpStorages.Remove(otpRecord);
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new { Status = "Success", StatusMessage = "Password reset successfully" });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", StatusMessage = result.Errors });
        }

    }
}
