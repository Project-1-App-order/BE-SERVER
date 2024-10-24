using api.Data;
using api.DTOs;
using api.Models;
using api.Responses;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> GetAllFoods()
        {
            var result = await _productService.GetFoodsAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllImagesAndFoods()
        {
            var result = await _productService.GetFoodImageAsync();
            return Ok(result);
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

    }
}
