using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Services.Interfaces
{
    public interface IProductService
    {
        Task<object> GetFoodsAsync(FoodDTO? foodDTO = null);
        Task<List<Category>> GetCategoryAsync();
    }
}
