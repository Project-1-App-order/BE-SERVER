using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<FoodDTO>> GetProductsAsync(string? id, string? name);
        Task<List<Category>> GetCategoryAsync();

    }
}
