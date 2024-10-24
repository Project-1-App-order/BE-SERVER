using api.Data;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        public CategoriesController(IProductService productService, ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var allCategories = await _productService.GetCategoryAsync();
            return Ok(allCategories);
        }
    }
}
