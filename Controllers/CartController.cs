using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] CartDTO cartDto)
        {
            var existUser = await _context.Orders.FindAsync(cartDto.UserId);
            if (existUser != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Already exist");
            }

            var newOrder = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                UserId = cartDto.UserId,
                OrderTypeId = "1",
                OrderStatus =  cartDto.OrderStatus.ToString(),
                OrderNote = cartDto.OrderNote,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
            };
            await _context.Orders.AddAsync(newOrder);
            return await _context.SaveChangesAsync() > 0 ? StatusCode(StatusCodes.Status200OK, "Success") : StatusCode(StatusCodes.Status500InternalServerError, "Error");
        }

       
        
        
    }
}