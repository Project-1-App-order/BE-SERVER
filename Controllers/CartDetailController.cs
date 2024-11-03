using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
            var existOrderDetail =   _context.OrderDetails.FirstOrDefault(x => x.OrderId == orderDetailDto.OrderId && x.FoodId == orderDetailDto.FoodId);
            if (existOrderDetail == null) return StatusCode(StatusCodes.Status404NotFound);
           
            existOrderDetail.Quantity = orderDetailDto.Quantity;
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
    }
}
