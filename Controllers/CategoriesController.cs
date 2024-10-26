using api.Data;
using api.DTOs;
using api.Models;
using api.Services.Functions;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.Responses;

namespace api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        public CategoriesController(IProductService productService, ApplicationDbContext context, IImageService imageService)
        {
            _productService = productService;
            _context = context;
            _imageService = imageService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var allCategories = await _productService.GetCategoryAsync();
            return Ok(allCategories);
        }


        [HttpPost]
        public async Task<IActionResult> UploadCategoriesImage([FromForm] CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (categoryDTO == null) return BadRequest("Category is null.");
            string imgPath = null;
            if (categoryDTO.formFile != null)
            {
                var uploadResult = await _imageService.AddImageAsync(categoryDTO.formFile);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    imgPath = uploadResult.SecureUrl.AbsoluteUri;
                }
                else
                {
                    return StatusCode((int)uploadResult.StatusCode, "Image upload failed.");
                }
            }
            var newCategory = new Category
            {
                CategoryId = Guid.NewGuid().ToString(),
                CategoryName = categoryDTO.CategoryName,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                CategoryImgUrl = imgPath
            };
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", StatusMessage = "Add category sucess" });
        }
    }
}
