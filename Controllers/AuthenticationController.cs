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
        public AuthenticationController(IAuthenticationService authenticationService, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMailService mailService)
        {
             _authenticationService = authenticationService;
             _context = context;
             _userManager = userManager;
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var result = await _authenticationService.ResgisterAsync(registerDTO);
            if (result.Flag == false)
            {
                StatusCode(StatusCodes.Status400BadRequest,
                                  new Response { Status = "Error", StatusMessage = result.Message });

            }
         
            return StatusCode(StatusCodes.Status200OK, new Response
            {
               Status = "Success",
               StatusMessage = "User created successfully"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authenticationService.LoginAsync(loginDTO);
            if (result.Flag == false)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", StatusMessage = result.Message });
            }
            return Ok(result.token);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changPasswordModel)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        public async Task<IActionResult> SendOTP(string email)
        {
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
                return NotFound("User not found");
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
        public async Task<IActionResult> VerifyOTP(string email, string otp)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { Status = "Error", StatusMessage = "User not found, email does not exist" });
            }
            var exignOtp = await _context.OtpStorages.FirstOrDefaultAsync(o => o.Otp == otp && o.UserId == user.Id);
            if (exignOtp == null)
            {
                return BadRequest();
            }
            if (exignOtp.ExpiryTime <= DateTime.UtcNow)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", StatusMessage = "OTP is expired" });
            }
            return StatusCode(StatusCodes.Status200OK, new { Status = "Success", StatusMessage = "OTP is valid" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetmodel)
        {
            var user = await _userManager.FindByEmailAsync(resetmodel.email);
            if (user == null)
                return NotFound("User not found");

            await _authenticationService.RemoveExpiredOtps();

            var otpRecord = await _context.OtpStorages.FirstOrDefaultAsync(o => o.UserId == user.Id && o.Otp == resetmodel.otp);
            if (otpRecord == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Status = "Error", StatusMessage = "OTP not found" });
            /*if (otpRecord.ExpiryTime <= DateTime.UtcNow)
                return BadRequest("OTP has expired");
*/
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetmodel.newPassword);

            if (result.Succeeded)
            {
                // Remove the used OTP
                _context.OtpStorages.Remove(otpRecord);
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new { Status = "Success", StatusMessage = "Password reset successfully" });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", StatusMessage = result.Errors });
        }



    }
}
