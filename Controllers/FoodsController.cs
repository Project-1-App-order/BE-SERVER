using api.Data;
using api.DTOs;
using api.Models;
using api.Responses;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Services.Functions;
using MySqlX.XDevAPI.Common;

namespace api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        public FoodsController(IProductService productService, ApplicationDbContext context, IImageService imageService)
        {
            _productService = productService;
            _context = context;
            _imageService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFoodsImages([FromForm] FoodImageDTO foodImageDTO)
        {
            if (foodImageDTO == null) return BadRequest("foodimagDTo is null.");
            string imgPath = null;
            if (foodImageDTO.formFile != null)
            {
                var uploadResult = await _imageService.AddImageAsync(foodImageDTO.formFile);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    imgPath = uploadResult.SecureUrl.AbsoluteUri;
                }
                else
                {
                    return StatusCode((int)uploadResult.StatusCode, "Image upload failed.");
                }
            }
            var newFoodImage = new FoodImage
            {
                ImageId = Guid.NewGuid().ToString(),
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                FoodId = foodImageDTO.FoodId,
                ImageUrl = imgPath,
            };
           
            _context.FoodImages.Add(newFoodImage);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", StatusMessage = "Add foodimg sucess" });
        }

        [HttpGet]
        public async Task<IActionResult> GetTopTenBestSeller()
        {
            var topFoodWithImages = await _context.Foods
                .Join(_context.OrderDetails,
                      f => f.FoodId,
                      od => od.FoodId,
                      (f, od) => new { Food = f, OrderDetail = od })
                .Join(_context.Orders,
                      fo => fo.OrderDetail.OrderId,
                      o => o.OrderId,
                      (fo, o) => new { fo.Food, Order = o, fo.OrderDetail })
                .Where(result => result.Order.OrderTypeId == "2")
                .GroupBy(result => new { result.Food.FoodId, result.Food.FoodName, result.Food.Price, result.Food.Description, result.Food.Status })
                .OrderByDescending(x => x.Sum(x => x.OrderDetail.Quantity))
                .Select(g => new
                {
                    g.Key.FoodId,
                    g.Key.FoodName,
                    g.Key.Price,
                    g.Key.Status,
                    g.Key.Description,
                    FoodImages = g.SelectMany(x => x.Food.Images!.Select(img => img.ImageUrl)).Distinct().ToList()
                })
                .Take(10)
                .AsNoTracking()
                .ToListAsync();

            return Ok(topFoodWithImages);
        }

        [HttpGet]
        public async Task<IActionResult> FilterGetFoods([FromQuery]FoodDTO foodDTO)
        {
            if (foodDTO.StartPrice.HasValue && !foodDTO.EndPrice.HasValue)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", StatusMessage = "Invalid EndPrice" });
            }
            if(!foodDTO.StartPrice.HasValue &&  foodDTO.EndPrice.HasValue) foodDTO.StartPrice = 0;

            if (foodDTO.StartPrice < 0 || foodDTO.EndPrice < 0 || foodDTO.StartPrice > foodDTO.EndPrice)
               return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", StatusMessage = "Invalid" });
            var result = await _productService.GetFoodsAsync(foodDTO);
            return Ok(result);
        }
    }
}
