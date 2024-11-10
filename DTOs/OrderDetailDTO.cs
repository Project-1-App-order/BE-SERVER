namespace api.DTOs;

public class OrderDetailDTO
{
    public required string OrderId { get; set; }
    public required string FoodId { get; set; }
    public int? Quantity { get; set; }
    public string? Note { get; set; }
}