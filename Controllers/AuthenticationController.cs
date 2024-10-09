using api.Data;
using api.DTOs.Authentication;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Responses;
using Microsoft.AspNetCore.Identity;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthenticationController(IAuthenticationService authenticationService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
             _authenticationService = authenticationService;
             _context = context;
             _userManager = userManager;
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




    }
}
