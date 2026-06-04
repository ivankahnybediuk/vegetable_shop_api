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

    public async Task GetProducts_ReturnsBadRequest_WhenPageSizeIsZero()
    {
        var productServiceMock = new Mock<IProductService>();
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProducts(1, 0);
        Assert.IsType<BadRequestResult>(actionResult);
    }
    
    public async Task GetProducts_ReturnsBadRequest_WhenPageIsZero()
    {
        var productServiceMock = new Mock<IProductService>();
        var controller = new ProductsController(productServiceMock.Object);
        var actionResult = await controller.GetProducts(0, 10);
        Assert.IsType<BadRequestResult>(actionResult);
    }
    
}