using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class FoodDTO
    {
        public string? FoodId { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]{5,50}$", ErrorMessage = "Invalid format.")]
        public string? FoodName { get; set; }
        public string? CategoryId { get; set; }
        //[Range(0, double.MaxValue, ErrorMessage = "Invalid")]
        [RegularExpression(@"^[0-9]*\.?[0-9]+$", ErrorMessage = "Invalid")]
        public decimal? StartPrice { get; set; }
        //[Range(0, double.MaxValue, ErrorMessage = "Invalid")]
        [RegularExpression(@"^[0-9]*\.?[0-9]+$", ErrorMessage = "Invalid")]
        public decimal? EndPrice { get; set; }
    }
}
