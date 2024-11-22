using api.Data;
using api.DTOs;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public async Task<Object> GetFoodsAsync(FoodDTO? foodDTO = null)
        {
            var query = _context.Foods
                                .Where(f =>
                                    (foodDTO == null || string.IsNullOrEmpty(foodDTO.FoodId) || f.FoodId == foodDTO.FoodId) &&
                                    (foodDTO == null || string.IsNullOrEmpty(foodDTO.FoodName) || f.FoodName.ToLower().Contains(foodDTO.FoodName.ToLower().Trim())) &&
                                    (foodDTO == null || string.IsNullOrEmpty(foodDTO.CategoryId) || f.CategoryId == foodDTO.CategoryId) &&
                                    (foodDTO == null || !foodDTO.StartPrice.HasValue && !foodDTO.EndPrice.HasValue ||  foodDTO.StartPrice <= f.Price && f.Price <= foodDTO.EndPrice  )
                                    )
                                    
                                .Include(f => f.Category)
                                .Select(f => new
                                {
                                    f.FoodId,
                                    f.FoodName,
                                    f.Price,
                                    f.Status,
                                    f.Description,
                                    FoodImages = _context.FoodImages
                                                        .Where(img => img.FoodId == f.FoodId)
                                                        .Select(img => img.ImageUrl)    
                                                        .ToList()
                                });

            var result = await query.ToListAsync();
            return new { result };
        }
        
        public async Task<object> GetCartAsync(string UserId , string OrderTypeId)
        {
            var query =  _context.Orders
                .Where(  o =>o.OrderTypeId == OrderTypeId && 
                                   o.UserId == UserId)
                .GroupJoin(
                    _context.OrderDetails,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, orderDetails) => new { o, orderDetails })
                .SelectMany(
                    x => x.orderDetails.DefaultIfEmpty(), // Left join với OrderDetails
                    (x, od) => new { x.o, OrderDetails = od })
                .GroupJoin(
                    _context.Foods,
                    j => j.OrderDetails.FoodId, // Đảm bảo lấy FoodId từ OrderDetails
                    f => f.FoodId,
                    (j, foods) => new { j.o, j.OrderDetails, Foods = foods })
                .SelectMany(
                    x => x.Foods.DefaultIfEmpty(),
                    (x, food) => new
                    {
                        x.o.OrderId,
                        x.o.OrderTypeId,
                        x.o.OrderDate,
                        x.o.UserId,
                        OrderTotal = (x.OrderDetails.Quantity ?? 0) * (food.Price ?? 0)
                    })
                .GroupBy(x => new { x.OrderId, x.OrderTypeId, x.OrderDate, x.UserId })
                .Select(g => new
                {
                    g.Key.OrderId,
                    g.Key.OrderTypeId,
                    g.Key.OrderDate,
                    g.Key.UserId,
                    OrderTotal = g.Sum(x => x.OrderTotal)
                });

            return query;
        }

    }
}
