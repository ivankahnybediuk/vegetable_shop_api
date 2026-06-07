using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using VegetableShopApi.Controllers;
using VegetableShopApi.DTOs.OrderDto;
using VegetableShopApi.DTOs.OrderItemDto;
using VegetableShopApi.DTOs.ProductDto;
using VegetableShopApi.Enums;
using VegetableShopApi.Models;
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
        
        var controller = new OrderController(orderServiceMock.Object, null);
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
        
        var controller = new OrderController(orderServiceMock.Object, null);
        var activeResult = await controller.CreateOrder(createDto);

        Assert.IsType<BadRequestObjectResult>(activeResult);
    }
    
    [Fact]
    public async Task CreateOrder_ReturnsBadRequest_WhenEmptyOrder()
    {
        var orderServiceMock = new Mock<IOrderService>();
        
        var controller = new OrderController(orderServiceMock.Object, null);
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
        
        var controller = new OrderController(orderServiceMock.Object, null);
        var activeResult = await controller.CreateOrder(createDto);

        Assert.IsType<NotFoundObjectResult>(activeResult);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOk_WithListOfOrders()
    {
        var orderServiceMock = new Mock<IOrderService>();
        orderServiceMock.Setup(s => s.GetAllAsync(1, 2))
            .ReturnsAsync(new PagedResult<OrderDto>(new List<OrderDto>(), 1,2, 1));

        var controller = new OrderController(orderServiceMock.Object, null);
        var actionResult = await controller.GetOrders(1, 2);
        
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var orders = Assert.IsType<PagedResult<OrderDto>>(okObjectResult.Value);
        Assert.Equal(1, orders.TotalCount);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsBadRequest_WhenPageIsZero()
    {
        var orderServiceMock = new Mock<IOrderService>();

        var controller = new OrderController(orderServiceMock.Object, null);
        var actionResult = await controller.GetOrders(0, 2);
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async Task UpdateOrder_ReturnsOk_WithOrderDto()
    {
        var orderServiceMock = new Mock<IOrderService>();
        int orderId = 1;
        OrderUpdateDto orderUpdateDto = new OrderUpdateDto() { Status = "confirmed" };
        OrderDto orderDto = new OrderDto(){ Id = orderId, Status = "confirmed" };
        orderServiceMock.Setup(s => s.UpdateOrderAsync(orderUpdateDto, orderId)).ReturnsAsync(orderDto);

        var controller = new OrderController(orderServiceMock.Object, null);
        var actionResult = await controller.UpdateOrder(orderUpdateDto, orderId);
        
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var order = Assert.IsType<OrderDto>(okObjectResult.Value);
        Assert.Equal(orderDto.Status, order.Status);
    }
    
    [Fact]
    public async Task UpdateOrder_ReturnsNotFound_WhenOrderDoesntExists()
    {
        var orderServiceMock = new Mock<IOrderService>();
        int orderId = 1;
        OrderUpdateDto orderUpdateDto = new OrderUpdateDto() { Status = "confirmed" };
        OrderDto orderDto = new OrderDto(){ Id = orderId, Status = "confirmed" };
        orderServiceMock.Setup(s => s.UpdateOrderAsync(orderUpdateDto, orderId))
            .ThrowsAsync(new KeyNotFoundException());

        var controller = new OrderController(orderServiceMock.Object, null);
        var actionResult = await controller.UpdateOrder(orderUpdateDto, orderId);
        
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WithOrderDetailesDto()
    {
        var orderServiceMock = new Mock<IOrderService>();
        orderServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new OrderDetailsDto());
        
        var controller = new OrderController(orderServiceMock.Object, null);
        var actionResult = await controller.GetById(1);
        
        var result = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(result.Value);
    }
    
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenOrderDoesntExists()
    {
        var orderServiceMock = new Mock<IOrderService>();
        orderServiceMock.Setup(s => s.GetByIdAsync(1)).ThrowsAsync(new KeyNotFoundException());
        
        var controller = new OrderController(orderServiceMock.Object, null);
        var actionResult = await controller.GetById(1);
        
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }
}