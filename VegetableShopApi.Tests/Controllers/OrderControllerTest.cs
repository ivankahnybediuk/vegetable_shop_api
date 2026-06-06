using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using VegetableShopApi.Controllers;
using VegetableShopApi.DTOs.OrderDto;
using VegetableShopApi.DTOs.OrderItemDto;
using VegetableShopApi.Services;
using VegetableShopApi.Services.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace VegetableShopApi.Tests.Controllers;

public class OrderControllerTest
{
    public static OrderCreateDto CreateOrderCreateDto()
    {
        return new OrderCreateDto()
        {
            CustomerName = "John",
            CustomerLastname = "Doe",
            CustomerEmail = "doe@email",
            CustomerPhone = "123456789",
            DeliveryAddress = "123 Main St",
            Items = new List<OrderItemCreateDto>()
            {
                new OrderItemCreateDto() { ProductId = 2, Quantity = 1.0m },
                new OrderItemCreateDto() { ProductId = 3, Quantity = 2.0m }
            }
        };
    }
    
    [Fact]
    public async Task CreateOrder_ReturnsOk_WithCreatedOrderDto()
    {
        OrderCreateDto createDto = CreateOrderCreateDto();
        
        var orderServiceMock = new Mock<IOrderService>();
        orderServiceMock.Setup(s => s.CreateOrderAsync(createDto))
            .ReturnsAsync(new OrderDto() { Id = 1, DeliveryAddress = createDto.DeliveryAddress });
        
        var controller = new OrderController(orderServiceMock.Object);
        var activeResult = await controller.CreateOrder(createDto);

        var okResult = Assert.IsType<OkObjectResult>(activeResult);
        var orderDto = Assert.IsType<OrderDto>(okResult.Value);
        Assert.NotNull(orderDto);
    }
    
    [Fact]
    public async Task CreateOrder_ReturnsBadRequest_WithInvalidOrderDto()
    {
        OrderCreateDto createDto = CreateOrderCreateDto();
        var orderServiceMock = new Mock<IOrderService>();
        orderServiceMock.Setup(s => s.CreateOrderAsync(createDto))
            .ThrowsAsync(new ArgumentException());
        
        var controller = new OrderController(orderServiceMock.Object);
        var activeResult = await controller.CreateOrder(createDto);

        Assert.IsType<BadRequestObjectResult>(activeResult);
    }
    
    [Fact]
    public async Task CreateOrder_ReturnsBadRequest_WhenEmptyOrder()
    {
        var orderServiceMock = new Mock<IOrderService>();
        
        var controller = new OrderController(orderServiceMock.Object);
        var activeResult = await controller.CreateOrder(null);

        Assert.IsType<BadRequestObjectResult>(activeResult);
    }
    
    [Fact]
    public async Task CreateOrder_ReturnsNotFound_WhenProductDoesntExists()
    {
        OrderCreateDto createDto = CreateOrderCreateDto();
        var orderServiceMock = new Mock<IOrderService>();
        orderServiceMock.Setup(s => s.CreateOrderAsync(createDto))
            .ThrowsAsync(new KeyNotFoundException());
        
        var controller = new OrderController(orderServiceMock.Object);
        var activeResult = await controller.CreateOrder(createDto);

        Assert.IsType<NotFoundObjectResult>(activeResult);
    }
}