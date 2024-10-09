using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class FoodImage
    {
        [Key]
        public string ImageId { get; set; }
        public string FoodId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        [ForeignKey("FoodId")]
        public Food Foods { get; set; }

    }
}
