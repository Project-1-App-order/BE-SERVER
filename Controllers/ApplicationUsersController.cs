using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ApplicationUsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ApplicationUsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { Status = "Error", StatusMessage = "User not found" });
            }
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == userName);
            return Ok(user);

        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile(ApplicationUserDTO user)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { Status = "Error", StatusMessage = "User not found" });
            }
            var userToUpdate = _userManager.Users.FirstOrDefault(x => x.UserName == userName);
            userToUpdate.FullName = user.FullName;
            userToUpdate.Address = user.Address;
            userToUpdate.Gender = user.Gender;
            userToUpdate.PhoneNumber = user.PhoneNumber;
           
            var result = await _userManager.UpdateAsync(userToUpdate);
            if (result.Succeeded)
            {
                return Ok(new { Status = "Success", StatusMessage = "User updated successfully" });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", StatusMessage = "User update failed" });
        }

    }
}
