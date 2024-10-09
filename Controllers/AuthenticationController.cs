using api.Data;
using api.DTOs.Authentication;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Responses;
using Microsoft.AspNetCore.Identity;
using api.Models;

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



    }
}
