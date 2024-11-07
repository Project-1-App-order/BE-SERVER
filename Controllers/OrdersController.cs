using System.Security.Claims;
using api.Data;
using api.DTOs;
using api.Models;
using api.Services.Functions;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager; 

        public OrdersController(ApplicationDbContext context, IProductService productService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _productService = productService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderDto)
        {
            var newOrder = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                UserId = orderDto.UserId,
                OrderTypeId = "2",
                OrderStatus = orderDto.OrderStatus.ToString(),
                OrderNote = orderDto.OrderNote,
                OrderDate = DateTime.Now,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
            };
            await _context.Orders.AddAsync(newOrder);
            return await _context.SaveChangesAsync() > 0 ? StatusCode(StatusCodes.Status200OK, "Success") : StatusCode(StatusCodes.Status500InternalServerError, "Error");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrdersByIdUser()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userName == null) return StatusCode(StatusCodes.Status401Unauthorized, "User not found");
            var user = await _userManager.FindByNameAsync(userName);
            var order = _productService.GetCartAsync(user.Id, "2");
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersDetailByOrderId(string orderId)
        {
            var orderDetails = await _context.OrderDetails.FindAsync(orderId);
            return Ok(orderDetails);
        }

    }
}
