using VegetableShopApi.DTOs.OrderDto;

namespace VegetableShopApi.Services.Interfaces;

public interface IOrderService
{
    public Task<OrderDto> CreateOrderAsync(OrderCreateDto orderDto);
}