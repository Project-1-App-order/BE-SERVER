using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class CategoryDTO
    {
        public required string CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
