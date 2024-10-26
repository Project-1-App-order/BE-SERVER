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

        public async Task<Object> GetFoodsAsync(string? id = null, string? name = null)
        {
            var query = _context.Foods
                                .Include(b => b.Category)
                                .Join(_context.FoodImages,
                                      f => f.FoodId,
                                      img => img.FoodId,
                                      (f, img) => new { Food = f, FoodImage = img }) 
                                .AsQueryable();
            if (!String.IsNullOrEmpty(id))
            {
                query = query.Where(b =>b.Food.FoodId == id);
            }
            if (!String.IsNullOrEmpty(name))
            {
                query = query.Where(f => f.Food.FoodName.ToLower().Contains(name.ToLower().Trim()));
            }
            var result = await query.GroupBy(query => new { query.Food.FoodId, query.Food.FoodName, query.Food.Price })
                                    .Select(g => new
                                    {
                                        FoodId = g.Key.FoodId,
                                        FoodName = g.Key.FoodName,
                                        Price = g.Key.Price,
                                        FoodImages = g.Select(f => f.FoodImage.ImageUrl).ToList()
                                    })
                                    .ToListAsync();
            return new { result };
        }
    }
}
