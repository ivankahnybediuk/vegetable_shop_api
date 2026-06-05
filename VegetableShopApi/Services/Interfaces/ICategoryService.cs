using VegetableShopApi.DTOs.ProductDto;

namespace DefaultNamespace;

public interface ICategoryService
{
    public Task<List<CategoryDto>> GetAllAsync();
}