using VegetableShopApi.Enums;
using VegetableShopApi.Models;
using VegetableShopApi.Services;
using VegetableShopApi.Tests.Helpers;

namespace VegetableShopApi.Tests.Services;

public class ProductServiceTests
{
    private static Category CreateCategory(int id)
    {
        return new Category
        {
            Id = id,
            Name = $"Vegetables"
        };
    }

    private static List<Product> CreateProducts(Category category)
    {
        return new()
        {
            new Product
            {
                Name = "Potato",
                Price = 20m,
                Unit = UnitType.Kg,
                StockQuantity = 100m,
                Category = category
            },
            new Product
            {
                Name = "Carrot",
                Price = 30m,
                Unit = UnitType.Kg,
                StockQuantity = 50m,
                Category = category
            }
        };
    }
    
    
    [Fact]
    public async Task GetAllPagedAsync_ReturnsAllProducts()
    {
        await using var context = TestDbContextFactory.Create();
        var category = CreateCategory(1);

        context.Categories.Add(category);

        context.Products.AddRange(CreateProducts(category));
        await context.SaveChangesAsync();

        var service = new ProductService(context);
        var result = await service.GetAllPaginatedAsync(1, 10);
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal("Potato", result.Items[0].Name);
        Assert.Equal("Vegetables", result.Items[0].CategoryName);
    }

    [Fact]
    public async Task GetAllPagedAsync_ReturnsEmptyListIfNoProducts()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new ProductService(context);
        var result = await service.GetAllPaginatedAsync(1, 10);
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsProductList()
    {
        await using var context = TestDbContextFactory.Create();
        var categories = new List<Category>() { CreateCategory(2), CreateCategory(1) };
        context.Categories.AddRange(categories);

        var listOfProducts = new List<Product>();
        listOfProducts.AddRange(CreateProducts(categories[0]));
        listOfProducts.AddRange(CreateProducts(categories[1]));
        context.Products.AddRange(listOfProducts);
        
        await context.SaveChangesAsync();
        var service = new ProductService(context);
        var result = await service.GetByCategoryIdAsync(1, 1, 2);
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(1, result.Items[0].CategoryId);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsEmptyListIfNoProducts()
    {
        await using var context = TestDbContextFactory.Create();
        
        context.Categories.Add(CreateCategory(1));
        await context.SaveChangesAsync();
        
        var service = new ProductService(context);
        var result = await service.GetByCategoryIdAsync(1, 1, 2);
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Items);
    }
    
    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsExceptionIfCategoryNotExists()
    {
        await using var context = TestDbContextFactory.Create();
        
        var service = new ProductService(context);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByCategoryIdAsync(1, 1, 2));
    }

    [Fact]
    public async Task GetByNameAsync_ReturnsProductList()
    {
        await using var context = TestDbContextFactory.Create();
        var products = CreateProducts(CreateCategory(1));
        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        var service = new ProductService(context);
        var result = await service.GetByNameAsync("pot", 1, 2);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal("Potato", result.Items[0].Name);
    }

    [Fact]
    public async Task GetByNameAsync_ReturnsEmptyListIfNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new ProductService(context);
        var result = await service.GetByNameAsync("potato", 1, 2);
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Items);
    }
}