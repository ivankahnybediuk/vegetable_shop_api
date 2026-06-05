using Microsoft.EntityFrameworkCore;
using VegetableShopApi.Data;
using VegetableShopApi.DTOs.ProductDto;
using VegetableShopApi.Services.Interfaces;

namespace VegetableShopApi.Services;

public class ProductService : IProductService
{
    private AppDbContext _dbContext;

    public ProductService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<PagedResult<ProductDto>> GetAllPaginatedAsync(int page, int pageSize)
    {
        int totalCount = await _dbContext.Products.CountAsync();
        var products = await this._dbContext.Products
            .Skip(pageSize * (page - 1)).Take(pageSize).Include(p => p.Category).ToListAsync();
        return new PagedResult<ProductDto>(products.Select(p => new ProductDto(p))
            .ToList(), page, pageSize, totalCount);
    }

    public async Task<PagedResult<ProductDto>> GetByCategoryIdAsync(int categoryId, int page, int pageSize)
    {
        var categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == categoryId);
        if (!categoryExists) throw new KeyNotFoundException($"The category with id {categoryId} does not exist.");
        
        int totalCount = await _dbContext.Products.Where(p => p.CategoryId == categoryId).CountAsync();
        var products = await _dbContext.Products.Where(p=> p.CategoryId == categoryId).Skip(pageSize * (page - 1))
            .Take(pageSize).Include(p => p.Category).ToListAsync();
        return new PagedResult<ProductDto>(products.Select(p => new ProductDto(p)).ToList(), page, pageSize, totalCount);
    }
}