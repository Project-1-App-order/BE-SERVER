using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CartDetailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartDetailController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> AddCartDetail([FromBody] OrderDetailDTO orderDetailDto)
        {
            bool isAvailable = _context.Foods.Any(f => f.FoodId == orderDetailDto.FoodId && f.Status == 1);
            if(!isAvailable) return StatusCode(StatusCodes.Status409Conflict);
            var newOderDetail = new OrderDetail
            {
                OrderId = orderDetailDto.OrderId,
                FoodId = orderDetailDto.FoodId,
                Quantity = orderDetailDto.Quantity,
                Note = orderDetailDto.Note
            };
            await _context.OrderDetails.AddAsync(newOderDetail);
            return await _context.SaveChangesAsync()  > 0 ?  StatusCode(StatusCodes.Status201Created) : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCartDetail([FromBody] OrderDetailDTO orderDetailDto)
        {
            if(!_context.Foods.Any(f => f.FoodId == orderDetailDto.FoodId && f.Status == 1))
                return StatusCode(StatusCodes.Status409Conflict);
            var existOrderDetail =   _context.OrderDetails.FirstOrDefault(x => x.OrderId == orderDetailDto.OrderId && x.FoodId == orderDetailDto.FoodId);
            if (existOrderDetail == null) return StatusCode(StatusCodes.Status404NotFound);
            
            existOrderDetail.Quantity +=  orderDetailDto.Quantity;
            existOrderDetail.Note = orderDetailDto.Note;
            return await _context.SaveChangesAsync()  > 0 ?  StatusCode(StatusCodes.Status201Created) : StatusCode(StatusCodes.Status500InternalServerError);
                   
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCartDetail( string orderId, string foodId)
        {
            var existOrderDetail = _context.OrderDetails.FirstOrDefault(x => x.OrderId == orderId && x.FoodId == foodId);
            if(existOrderDetail == null) return StatusCode(StatusCodes.Status404NotFound);
             _context.OrderDetails.Remove(existOrderDetail);
             return await _context.SaveChangesAsync()  > 0 ?  StatusCode(StatusCodes.Status201Created) : StatusCode(StatusCodes.Status500InternalServerError);
        }

        
        [HttpDelete]
        public async Task<IActionResult> DeleteAllOrderDetailByOrderId(string orderId)
        {
            var orderDetails = _context.OrderDetails.Where(x => x.OrderId == orderId).ToList();
    
            if (!orderDetails.Any())
                return StatusCode(StatusCodes.Status404NotFound);

            _context.OrderDetails.RemoveRange(orderDetails);

            return await _context.SaveChangesAsync() > 0 
                ? StatusCode(StatusCodes.Status200OK) 
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCartDetailByCartId(string cartId)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderId == cartId)
                .Select(od => new
                {
                    od.Food.FoodId,
                    od.Food.FoodName,
                    od.Food.Price,
                    od.Quantity,
                    od.Note,
                    Images = od.Food.Images!.Select(img => img.ImageUrl).Distinct().ToList()
                })
                .ToListAsync();
            return Ok(orderDetails);
        }
    }
}
