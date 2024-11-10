using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Services.Interfaces
{
    public interface IProductService
    {
        Task<Object> GetFoodsAsync(FoodDTO? foodDTO = null);
        Task<List<Category>> GetCategoryAsync();
        public Task<object> GetCartAsync(string UserId , string OrderTypeId);

    }
}
