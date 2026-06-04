using VegetableShopApi.Enums;
using VegetableShopApi.Models;
using VegetableShopApi.Services;
using VegetableShopApi.Tests.Helpers;

namespace VegetableShopApi.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllPagedAsync_ReturnsAllProducts()
    {
        var context = TestDbContextFactory.Create();
        var category = new Category
        {
            Name = "Vegetables"
        };

        context.Categories.Add(category);

        context.Products.AddRange(
            new Product
            {
                Name = "Potato",
                Price = 20.0m,
                Unit = UnitType.Kg,
                StockQuantity = 100.0m,
                Category = category
            },
            new Product
            {
                Name = "Carrot",
                Price = 30.0m,
                Unit = UnitType.Kg,
                StockQuantity = 50.0m,
                Category = category
            });
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
        var context = TestDbContextFactory.Create();
        var service = new ProductService(context);
        var result = await service.GetAllPaginatedAsync(1, 10);
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Items);
    }
}