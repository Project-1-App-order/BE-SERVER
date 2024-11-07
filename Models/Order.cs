using MailKit.Search;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Order
    {
        [Key]
        public required string OrderId { get; set; }
        public required string UserId { get; set; }
        public required string OrderTypeId { get; set; }
        public required string OrderStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? OrderTotal { get; set; }
        public string? OrderNote { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        [ForeignKey("OrderTypeId")]
        public  OrderType? OrderType { get; set; }
        [ForeignKey("UserId")]
        public  ApplicationUser? User { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
