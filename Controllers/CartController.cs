using System.Security.Claims;
using api.Data;
using api.DTOs;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductService _productService;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,  IProductService productService)
        {
            _context = context;
            _userManager = userManager;
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] OrderDTO orderDto)
        {
            var existUser =  _context.Orders.FirstOrDefault(x => x.UserId == orderDto.UserId);
            if (existUser != null) return StatusCode(StatusCodes.Status409Conflict, "User already have cart");

            var newOrder = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                UserId = orderDto.UserId,
                OrderTypeId = "1",
                OrderStatus = null,
                OrderNote = orderDto.OrderNote,
                OrderDate = DateTime.Now,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
            };
            await _context.Orders.AddAsync(newOrder);
            return await _context.SaveChangesAsync() > 0 ? StatusCode(StatusCodes.Status200OK, "Success") : StatusCode(StatusCodes.Status500InternalServerError, "Error");
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userName == null) return StatusCode(StatusCodes.Status401Unauthorized, "User not found");
            var user = await _userManager.FindByNameAsync(userName);
            var cart = _productService.GetCartAsync(user.Id, "1");
            return Ok(cart);
        }
        
        
        
    }
}