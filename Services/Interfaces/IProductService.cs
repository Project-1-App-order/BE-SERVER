using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Services.Interfaces
{
    public interface IProductService
    {
        Task<Object> GetFoodsAsync(string? id = null, string? name = null);
        Task<List<Category>> GetCategoryAsync();
        Task<List<FoodImage>> GetFoodImageAsync(string? foodId = null);

    }
}
