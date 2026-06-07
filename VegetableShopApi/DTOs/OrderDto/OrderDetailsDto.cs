using VegetableShopApi.Models;

namespace VegetableShopApi.DTOs.OrderDto;

public class OrderDetailsDto : OrderDto
{
    public List<OrderItemDto.OrderItemDto> Items { get; set; } = new List<OrderItemDto.OrderItemDto>();

    public OrderDetailsDto()
    {
    }
    
    public OrderDetailsDto(Order order) : base(order)
    {
        Items = order.Items.Select(oi => new OrderItemDto.OrderItemDto(oi)).ToList();
    }
}