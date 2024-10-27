using api.Data;
using api.DTOs;
using api.Models;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.Services.Functions
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetCategoryAsync()
        {
            var query = _context.Categories
                               .AsQueryable();
            var result = await query.AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<object> GetFoodsAsync(FoodDTO? foodDTO = null)
        {
            var query = _context.Foods
                                .Where(f =>
                                    (string.IsNullOrEmpty(foodDTO.FoodId) || f.FoodId == foodDTO.FoodId) &&
                                    (string.IsNullOrEmpty(foodDTO.FoodName) || f.FoodName.ToLower().Contains(foodDTO.FoodName.ToLower().Trim())) &&
                                    (string.IsNullOrEmpty(foodDTO.CategoryId) || f.CategoryId == foodDTO.CategoryId))
                                .Include(f => f.Category)
                                .Select(f => new
                                {
                                    f.FoodId,
                                    f.FoodName,
                                    f.Price,
                                    FoodImages = _context.FoodImages
                                                        .Where(img => img.FoodId == f.FoodId)
                                                        .Select(img => img.ImageUrl)
                                                        .ToList()
                                });

            var result = await query.ToListAsync();
            return new { result };
        }

    }
}
