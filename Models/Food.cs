using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace api.Models
{
    public class Food
    {
        [Key]
        public string FoodId { get; set; }
        public string FoodName { get; set; }
        public string CategoryId { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string Description { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public ICollection<FoodImage>? Images { get; set; }

    }
}
