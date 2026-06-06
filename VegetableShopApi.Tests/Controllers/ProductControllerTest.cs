using Microsoft.AspNetCore.Mvc;
using Moq;
using VegetableShopApi.Controllers;
using VegetableShopApi.DTOs.ProductDto;
using VegetableShopApi.Enums;
using VegetableShopApi.Models;
using VegetableShopApi.Services.Interfaces;

namespace VegetableShopApi.Tests.Controllers;

public class ProductControllerTest
{
    [Fact]
    public async Task GetProducts_ReturnsOk_WithPaginatedListOfProducts()
    {
        var products = new List<ProductDto>
        {
            new ProductDto(new Product() {
                Id = 1,
                Name = "Potato",
                Price = 20m,
                Unit = UnitType.Kg,
                StockQuantity = 100m,
                ImageUrl = null,
                CategoryId = 1,
                Category = new Category() { Name = "Vegetables" }
            })
        };

        var pagedResult = new PagedResult<ProductDto>(
            products,
            page: 1,
            pageSize: 10,
            totalCount: 1
        );
        
        var productServiceMock = new Mock<IProductService>();
        
        productServiceMock
            .Setup(s => s.GetAllPaginatedAsync(1, 10))
            .ReturnsAsync(pagedResult);
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProducts(1, 10);
        
        Assert.IsType<OkObjectResult>(actionResult);
        var okObjectResult = (OkObjectResult)actionResult;
        var productDto = Assert.IsType<PagedResult<ProductDto>>(okObjectResult.Value);
        Assert.Equal(pagedResult.TotalCount, productDto.TotalCount);
        Assert.Equal(pagedResult.PageSize, productDto.PageSize);
        Assert.Equal(1, productDto.TotalPages);
    }

    [Fact]
    public async Task GetProducts_ReturnsBadRequest_WhenPageSizeIsZero()
    {
        var productServiceMock = new Mock<IProductService>();
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProducts(1, 0);
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }
    
    [Fact]
    public async Task GetProducts_ReturnsBadRequest_WhenPageIsZero()
    {
        var productServiceMock = new Mock<IProductService>();
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProducts(0, 10);
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async Task GetProductsByCategoryId_ReturnsOk_WithPaginatedListOfProductsByCategoryId()
    {
        var products = new List<ProductDto>
        {
            new ProductDto(new Product() {
                Id = 1,
                Name = "Potato",
                Price = 20m,
                Unit = UnitType.Kg,
                StockQuantity = 100m,
                ImageUrl = null,
                CategoryId = 1,
                Category = new Category() { Id = 1, Name = "Vegetables" }
            }), 
        };

        var pagedResult = new PagedResult<ProductDto>(
            products,
            page: 1,
            pageSize: 10,
            totalCount: 1
        );
        
        var productServiceMock = new Mock<IProductService>();
        
        productServiceMock
            .Setup(s => s.GetByCategoryIdAsync(1,1, 10))
            .ReturnsAsync(pagedResult);
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProductsByCategoryId(1,1, 10);
        
        Assert.IsType<OkObjectResult>(actionResult);
        var okObjectResult = (OkObjectResult)actionResult;
        var productDto = Assert.IsType<PagedResult<ProductDto>>(okObjectResult.Value);
        Assert.Equal(pagedResult.TotalCount, productDto.TotalCount);
        Assert.Equal(pagedResult.PageSize, productDto.PageSize);
        Assert.Equal(1, productDto.TotalPages);
    }

    [Fact]
    public async Task GetProductsByCategoryId_ReturnsNotFound_WhenCategoryDoesntExist()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(s => s.GetByCategoryIdAsync(1, 1, 10)).Throws<KeyNotFoundException>();
        var controller = new ProductsController(productServiceMock.Object);
        
        var actionResult = await controller.GetProductsByCategoryId(1,1, 10);
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }
    
    [Fact]
    public async Task GetProductsByCategoryId_ReturnsBadRequest_WhenPageIsZero()
    {
        var productServiceMock = new Mock<IProductService>();
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProductsByCategoryId(1,0, 10);
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async Task GetProductsByName_ReturnsOk_WithPaginatedListOfProducts()
    {
        var productServiceMock = new Mock<IProductService>();
        var products = new List<ProductDto>
        {
            new ProductDto(new Product() 
            {
                Id = 1,
                Name = "Potato",
                Price = 20m,
                Unit = UnitType.Kg,
                StockQuantity = 100m,
                ImageUrl = null,
                CategoryId = 1,
                Category = new Category() { Id = 1, Name = "Vegetables" }
            })
        };
        productServiceMock.Setup(s => s.GetByNameAsync("Potato", 1, 2))
            .ReturnsAsync(new PagedResult<ProductDto>(products, 1, 2, 1));
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProductsByName("Potato", 1, 2);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult);
        var pagedResult = Assert.IsType<PagedResult<ProductDto>>(objectResult.Value);
        Assert.Equal(1, pagedResult.TotalCount);
        Assert.Equal("Potato", pagedResult.Items[0].Name);
    }
    
    [Fact]
    public async Task GetProductsByName_ReturnsBadRequest_WhenPageIsZero()
    {
        var productServiceMock = new Mock<IProductService>();
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProducts(0, 10);
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }
    
    [Fact]
    public async Task GetProductsByName_ReturnsBadRequest_WhenNameIsLessThanThreeCharacters()
    {
        var productServiceMock = new Mock<IProductService>();
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProductsByName("Po", 1, 10);
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }
    
    
    
}