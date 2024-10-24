using api.Data;
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
        public FoodsController(IProductService productService, ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
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

    }
}
