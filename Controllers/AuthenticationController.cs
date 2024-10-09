using api.Data;
using api.DTOs.Authentication;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Responses;
using Microsoft.AspNetCore.Identity;
using api.Models;
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
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var userToken = _context.UserTokens.FirstOrDefault(ut => ut.Value == currentToken);

            if (userToken != null)
            {
                _context.UserTokens.Remove(userToken);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Logged out successfully from current device" });

            }
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", StatusMessage = "Logout failed" });
        }

    }
}
