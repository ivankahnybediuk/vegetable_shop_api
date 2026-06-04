using VegetableShopApi.DTOs.ProductDto;
using VegetableShopApi.Models;

namespace VegetableShopApi.Services.Interfaces;

public interface IProductService
{
    public Task<PagedResult<ProductDto>> GetAllPaginatedAsync(int page, int pageSize);
}