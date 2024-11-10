using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class FoodDTO
    {
        public string? FoodId { get; set; }

        public string? FoodName { get; set; }
        
        public string? CategoryId { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid")]
        public decimal? StartPrice { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid")]
        public decimal? EndPrice { get; set; }
    }
}
