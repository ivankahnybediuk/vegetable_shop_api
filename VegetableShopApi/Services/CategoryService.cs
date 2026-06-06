using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using VegetableShopApi.Data;

namespace VegetableShopApi.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _dbContext;

    public CategoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _dbContext.Categories.OrderBy(c => c.Id).ToListAsync();
        return categories.Select(c => new CategoryDto(c)).ToList();
    }
}