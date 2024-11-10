using System.Security.Claims;
using api.Data;
using api.DTOs;
using api.Models;
using api.Services.Functions;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost]
        public async Task<IActionResult> AddAndDeleteOrderDetail([FromBody] OrderDetailDTO orderDetailDto, string? detailCartIdDelete = null)
        {
            // Khởi tạo transaction để đảm bảo tính toàn vẹn của dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Thêm OrderDetail mới
                var newOrderDetail = new OrderDetail
                {
                    OrderId = orderDetailDto.OrderId,
                    FoodId = orderDetailDto.FoodId,
                    Quantity = orderDetailDto.Quantity,
                    Note = orderDetailDto.Note
                };

                await _context.OrderDetails.AddAsync(newOrderDetail);
                await _context.SaveChangesAsync();

                // 2. Tìm và xóa chi tiết trong giỏ hàng từ bảng OrderDetails và Orders
                var cartItem = _context.OrderDetails
                    .Join(_context.Orders,
                        od => od.OrderId,
                        o => o.OrderId,
                        (od, o) => new { OrderDetails = od, Orders = o })
                    .FirstOrDefault(c => c.OrderDetails.FoodId == orderDetailDto.FoodId 
                                         && c.Orders.OrderId == detailCartIdDelete 
                                         && c.Orders.OrderTypeId == "1");

                if (cartItem != null)
                {
                    _context.OrderDetails.Remove(cartItem.OrderDetails);
                    await _context.SaveChangesAsync();
                }

                // 3. Commit transaction nếu cả hai thao tác đều thành công
                await transaction.CommitAsync();
                return StatusCode(StatusCodes.Status200OK, "Order detail added and cart item removed.");
            }
            catch (Exception ex)
            {
                // Rollback nếu có lỗi
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
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
            var orderDetails = await  _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
            return Ok(orderDetails);
        }

    }
}
