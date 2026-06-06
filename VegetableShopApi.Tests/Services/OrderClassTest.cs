using Microsoft.EntityFrameworkCore;
using VegetableShopApi.DTOs.OrderDto;
using VegetableShopApi.DTOs.OrderItemDto;
using VegetableShopApi.Exceptions;
using VegetableShopApi.Models;
using VegetableShopApi.Services;
using VegetableShopApi.Tests.Helpers;

namespace VegetableShopApi.Tests.Services;

public class OrderClassTest
{
    [Fact]
    public async Task CreateOrderAsync_ThrowsArgumentException_WhenEmptyItems()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);

        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>(),
            CustomerName = "Test",
            CustomerLastname = "Test",
            CustomerEmail = "<EMAIL>",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };
        
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(orderDto));
    }
    
    [Fact]
    public async Task CreateOrderAsync_ThrowsArgumentException_WhenEmptyFirstName()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);

        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>() { new OrderItemCreateDto() { ProductId = 1, Quantity = 2 } },
            CustomerName = "  ",
            CustomerLastname = "Test",
            CustomerEmail = "<EMAIL>",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };
        
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(orderDto));
    }
    
    [Fact]
    public async Task CreateOrderAsync_ThrowsArgumentException_WhenQuantityIsZero()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);

        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>() { new OrderItemCreateDto() { ProductId = 1, Quantity = 0 } },
            CustomerName = "Name",
            CustomerLastname = "Test",
            CustomerEmail = "<EMAIL>",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };
        
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(orderDto));
    }
    
    [Fact]
    public async Task CreateOrderAsync_ThrowsKeyNotFound_WhenProductDoesntExists()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);
        context.Products.Add(new Product() { Id = 1, Name = "Test" });
        await context.SaveChangesAsync();
        
        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>() { new OrderItemCreateDto() { ProductId = 2, Quantity = 2 } },
            CustomerName = "Name",
            CustomerLastname = "Test",
            CustomerEmail = "<EMAIL>",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateOrderAsync(orderDto));
    }
    
    [Fact]
    public async Task CreateOrderAsync_ThrowsInsufficientStock_WhenProductDoesntHaveEnoughStock()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);
        context.Products.Add(new Product() { Id = 1, Name = "Test", StockQuantity = 10.0m });
        await context.SaveChangesAsync();
        
        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>() { new OrderItemCreateDto() { ProductId = 1, Quantity = 20 } },
            CustomerName = "Name",
            CustomerLastname = "Test",
            CustomerEmail = "<EMAIL>",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };
        
        await Assert.ThrowsAsync<InsufficientStockException>(() => service.CreateOrderAsync(orderDto));
    }
    
    [Fact]
    public async Task CreateOrderAsync_ThrowsInsufficientStock_WhenProductDoesntHaveEnoughStockInSummForOrder()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);
        context.Products.Add(new Product() { Id = 1, Name = "Test", StockQuantity = 10.0m });
        await context.SaveChangesAsync();
        
        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>()
            {
                new OrderItemCreateDto() { ProductId = 1, Quantity = 2 },
                new OrderItemCreateDto() { ProductId = 1, Quantity = 9 }
            },
            CustomerName = "Name",
            CustomerLastname = "Test",
            CustomerEmail = "<EMAIL>",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };
        
        await Assert.ThrowsAsync<InsufficientStockException>(() => service.CreateOrderAsync(orderDto));
    }
    
    [Fact]
    public async Task CreateOrderAsync_UseSavedUser_IfThereIsAlreadyUserWithSameEmail()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);
        context.Users.Add(new User()
        {
            Email = "test@email.com",
            FirstName = "TestNameBefore",
            LastName = "TestLastName",
            Phone = "123456789"
        });
        context.Products.Add(new Product() { Id = 1, Name = "Test", StockQuantity = 10.0m });
        await context.SaveChangesAsync();
        
        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>()
            {
                new OrderItemCreateDto() { ProductId = 1, Quantity = 2 }
            },
            CustomerName = "TestName",
            CustomerLastname = "TestLastName",
            CustomerEmail = "Test@email.com ",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };
        int usersWithSameEmail = await context.Users.CountAsync();

        await service.CreateOrderAsync(orderDto);
        
        int usersWithSameEmailAfter = await context.Users.CountAsync();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@email.com");
        Assert.Equal(usersWithSameEmail, usersWithSameEmailAfter);
        Assert.Equal("TestNameBefore", user.FirstName);
        Assert.Equal("TestLastName", user.LastName);
        Assert.Equal("123456789", user.Phone);
    }
    
    [Fact]
    public async Task CreateOrderAsync_CreateUser_IfThereIsNoUserWithSameEmail()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);
        context.Users.Add(new User()
        {
            Email = "test@email.com",
            FirstName = "TestNameBefore",
            LastName = "TestLastName",
            Phone = "123456789"
        });
        context.Products.Add(new Product() { Id = 1, Name = "Test", StockQuantity = 10.0m });
        await context.SaveChangesAsync();
        
        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>()
            {
                new OrderItemCreateDto() { ProductId = 1, Quantity = 2 }
            },
            CustomerName = "TestName",
            CustomerLastname = "TestLastName",
            CustomerEmail = " testSecond@email.com ",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };
        int usersInDbBefore = await context.Users.CountAsync();

        var createdOrder = await service.CreateOrderAsync(orderDto);
        
        int usersInDbAfter = await context.Users.CountAsync();
        
        Assert.Equal(1, usersInDbBefore);
        Assert.Equal(2, usersInDbAfter);
        Assert.Equal("TestName TestLastName", createdOrder.Customer.Name);
        Assert.Equal("testsecond@email.com", createdOrder.Customer.Email);
    }
    
    [Fact]
    public async Task CreateOrderAsync_ReturnsOrderDtoStoreToDbAndChangesStockQuantity_WhenOrderIsCreated()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);
       
        context.Products.Add(new Product() { Id = 1, Name = "Test", StockQuantity = 10.0m, Price = 2.0m });
        await context.SaveChangesAsync();
        
        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>()
            {
                new OrderItemCreateDto() { ProductId = 1, Quantity = 2 }
            },
            CustomerName = "TestName",
            CustomerLastname = "TestLastName",
            CustomerEmail = " testSecond@email.com ",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };

        var createdOrder = await service.CreateOrderAsync(orderDto);
        
        int ordersInDbAfter = await context.Users.CountAsync();
        int orderItemsInDbAfter = await context.OrderItems.CountAsync();
        decimal stockQuantityAfter = context.Products.First().StockQuantity;
        
        Assert.Equal(1, ordersInDbAfter);
        Assert.Equal(1, orderItemsInDbAfter);
        Assert.Equal(8.0m, stockQuantityAfter);
        Assert.Equal(4.0m, createdOrder.Total);
    }
    
    
    
    
    
    
}