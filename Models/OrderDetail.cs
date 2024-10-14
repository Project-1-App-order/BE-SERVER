using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class OrderDetail
    {
        [Key]
        public required string OrderId { get; set; }
        public string? FoodId { get; set; }
        public int? Quantity { get; set; }
        public string? Note { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        [ForeignKey("FoodId")]
        public Food? Food { get; set; }
        [ForeignKey("OrderId")]
        public required Order Order { get; set; }
        
    }
}
