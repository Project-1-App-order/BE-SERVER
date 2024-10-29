﻿using api.Data;
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
                                    f.Description,
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
