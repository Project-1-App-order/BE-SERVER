using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class OrderType
    {
        [Key]
        public required string OderTypeId { get; set; }
        public string? OderTypeName { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public ICollection<Order>? Orders { get; set; }


    }
}
