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

        public Task<IEnumerable<FoodDTO>> GetProductsAsync(string? id, string? name)
        {
            throw new NotImplementedException();
        }
    }
}
