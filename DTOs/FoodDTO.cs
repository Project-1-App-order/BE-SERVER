using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class FoodDTO
    {
        public string? FoodId { get; set; }
        public string? FoodName { get; set; }
        public string? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Description { get; set; }
    }
}
