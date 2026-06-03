using VegetableShopApi.DTOs.OrderItemDto;

namespace VegetableShopApi.DTOs.OrderDto;

public class OrderCreateDto
{
    public required string CustomerName { get; set; }
    public required string CustomerLastname { get; set; }
    public required string CustomerEmail { get; set; }
    public required string CustomerPhone { get; set; }
    public required string DeliveryAddress { get; set; }
    public List<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();
}