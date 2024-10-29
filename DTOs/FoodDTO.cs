using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class FoodDTO
    {
        public string? FoodId { get; set; }
        public string? FoodName { get; set; }
        public string? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public decimal? StartPrice { get; set; }
        public decimal? EndPrice { get; set; }
    }
}
