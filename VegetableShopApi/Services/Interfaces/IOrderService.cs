using VegetableShopApi.DTOs.OrderDto;
using VegetableShopApi.DTOs.ProductDto;

namespace VegetableShopApi.Services.Interfaces;

public interface IOrderService
{
    public Task<OrderDto> CreateOrderAsync(OrderCreateDto orderDto);
    public Task<PagedResult<OrderDto>> GetAllAsync(int page, int pageSize);
}