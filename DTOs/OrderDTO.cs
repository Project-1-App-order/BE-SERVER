namespace api.DTOs;

public class OrderDTO
{
    public required string UserId { get; set; }
    public required OrderStatus OrderStatus { get; set; }
    public  string? OrderNote { get; set; }
    
    
}

public enum OrderStatus
{
    Pending,
    InProgress,
    Done
    
}