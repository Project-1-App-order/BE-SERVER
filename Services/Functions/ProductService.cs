using api.Data;
using api.DTOs;
using api.Models;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<FoodImage>> GetFoodImageAsync(string? foodId = null)
        {
            var query = _context.FoodImages
                                .Include(b => b.Foods)
                                .AsQueryable();
            if (!string.IsNullOrEmpty(foodId))
            {
                query = query.Where(b => b.FoodId == foodId);
            }
            var result = await query.AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<Object> GetFoodsAsync(string? id = null, string? name = null)
        {
            var query = _context.Foods
                                .Include(b => b.Category)
                                .AsQueryable();
            if (!String.IsNullOrEmpty(id))
            {
                query = query.Where(b =>b.Equals(id));
            }
            if (!String.IsNullOrEmpty(name))
            {
                query = query.Where(b => b.FoodName.ToLower().Contains(name.ToLower().Trim()));
            }
            var result = await query.AsNoTracking().ToListAsync();
            return new { result };
        }
    }
}
