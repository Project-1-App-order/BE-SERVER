using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class CategoryDTO
    {
        public string? CategoryName { get; set; }
        public IFormFile? formFile { get; set; }
    }
}
