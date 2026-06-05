using DefaultNamespace;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VegetableShopApi.Controllers;
using VegetableShopApi.Models;

namespace VegetableShopApi.Tests.Controllers;

public class CategoryControllerTest
{
    [Fact]
    public async Task GetCategories_ReturnsOk_WithListOfCategories()
    {
        List<CategoryDto> categories = new List<CategoryDto>()
        {
            new CategoryDto(new Category() { Id = 1, Name = "Category 1" }),
            new CategoryDto(new Category() { Id = 2, Name = "Category 2" })
        };
        var mockCategoryService = new Mock<ICategoryService>();
        mockCategoryService.Setup(s => s.GetAllAsync()).ReturnsAsync(categories);

        var controller = new CategoryController(mockCategoryService.Object);
        var actionResult = await controller.GetAllCategories();
        ;
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var categoriesDto = Assert.IsType<List<CategoryDto>>(okObjectResult.Value);
        Assert.Equal(2, categoriesDto.Count);
    }
    
    [Fact]
    public async Task GetCategories_ReturnsOk_WithEmptyListOfCategories()
    {
        var mockCategoryService = new Mock<ICategoryService>();
        mockCategoryService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<CategoryDto>());

        var controller = new CategoryController(mockCategoryService.Object);
        var actionResult = await controller.GetAllCategories();
        ;
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var categories = Assert.IsType<List<CategoryDto>>(okObjectResult.Value);
        Assert.Empty(categories);
    }
}