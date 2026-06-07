using VegetableShopApi.Models;

namespace VegetableShopApi.DTOs.OrderDto;

public class OrderDto
{
    public int Id { get; set; }
    public UserDto.UserDto? Customer { get; set; }
    public string? DeliveryAddress { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Status { get; set; }
    public decimal Total { get; set; }

    public OrderDto()
    {
        
    }
    
    public OrderDto(Order order)
    {
        Id = order.Id;
        Customer = new UserDto.UserDto(order.User);
        DeliveryAddress = order.DeliveryAddress;
        CreatedAt = order.CreatedAt;
        Status = order.Status.ToString().ToLower();
        Total = order.TotalPrice;
    }
}