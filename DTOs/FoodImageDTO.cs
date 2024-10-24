using api.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.DTOs
{
    public class FoodImageDTO
    {
        public required string FoodId { get; set; }
        public required IFormFile formFile { get; set; }
    }
}
