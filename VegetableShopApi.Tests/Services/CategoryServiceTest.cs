using DefaultNamespace;
using VegetableShopApi.Data;
using VegetableShopApi.Models;
using VegetableShopApi.Services;
using VegetableShopApi.Tests.Helpers;

namespace VegetableShopApi.Tests.Services;

public class CategoryServiceTest
{

    [Fact]
    public async Task GetAllAsync_RetunsListOfCategoryDtoInCorrectOrder()
    {
        await using var context = TestDbContextFactory.Create();
        
        List<Category> categories = new List<Category>()
        {
            new Category(){ Id = 2, Name = "Category 2" },
            new Category(){ Id = 1, Name = "Category 1" }
        };
        context.AddRange(categories);
        context.SaveChangesAsync();

        var service = new CategoryService(context);
        var result = await service.GetAllAsync();
        
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Category 1", result[0].Name);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Category 2", result[1].Name);
    }
    
    [Fact]
    public async Task GetAllAsync_RetunsEmptyList()
    {
        await using var context = TestDbContextFactory.Create();

        var service = new CategoryService(context);
        var result = await service.GetAllAsync();
        
        Assert.Empty(result);
    }
    
}