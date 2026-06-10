using Microsoft.EntityFrameworkCore;
using VegetableShopApi.DTOs.OrderDto;
using VegetableShopApi.DTOs.OrderItemDto;
using VegetableShopApi.Enums;
using VegetableShopApi.Exceptions;
using VegetableShopApi.Models;
using VegetableShopApi.Services;
using VegetableShopApi.Tests.Helpers;

namespace VegetableShopApi.Tests.Services;

public class OrderClassTest
{

    private static List<Order> CreateOrders()
    {
        return new List<Order>()
        {
            new Order()
            {
                Id = 1,
                CreatedAt = DateTime.Parse("2026-01-10"),
                DeliveryAddress = "Bandery st. 3",
                User = new User()
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "<EMAIL>",
                    Phone = "123456789"
                },
                Status = OrderStatus.Pending,
                TotalPrice = 12.22m
            },
            new Order()
            {
                Id = 2,
                CreatedAt = DateTime.Parse("2026-01-01"),
                DeliveryAddress = "Shevchenka st. 25",
                User = new User()
                {
                    Id = 2,
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "<EMAIL>",
                    Phone = "123456789"
                },
                Status = OrderStatus.Delivered,
                TotalPrice = 45.12m
            }
        };
    }
    
    private static OrderItem CreateOrderItem(Order order, Product product, int id = 1)
    {
        return new OrderItem()
        {
            Id = id,
            ProductId = 1,
            Quantity = 2.0m,
            Order = order,
            Product = product
        };
    }

    private static Product CreateProduct()
    {
        return new Product()
        {
            Id = 1,
            Name = "Test",
            Price = 2.0m,
            StockQuantity = 10.0m
        };
    }
    
    
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
    
    [Fact]
    public async Task CreateOrderAsync_ThrowsArgumentException_WhenIsNotAWholeNumberQuantityForPcs()
    {
        using var context = TestDbContextFactory.Create();
        var service = new OrderService(context);
       
        context.Products.Add(new Product() { Id = 1, Name = "Test", StockQuantity = 10.0m, Price = 2.0m, Unit = UnitType.Pcs });
        await context.SaveChangesAsync();
        
        OrderCreateDto orderDto = new OrderCreateDto()
        {
            Items = new List<OrderItemCreateDto>()
            {
                new OrderItemCreateDto() { ProductId = 1, Quantity = 2.5m }
            },
            CustomerName = "TestName",
            CustomerLastname = "TestLastName",
            CustomerEmail = " testSecond@email.com ",
            CustomerPhone = "123456789",
            DeliveryAddress = "Test Address"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(orderDto));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var orders = CreateOrders();
        using var context = TestDbContextFactory.Create();
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();
        
        var service = new OrderService(context);
        var result = await service.GetAllAsync(1, 10);
        
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(1, result.Items[0].Id);
    }
    [Fact]
    public async Task GetAllAsync_ReturnsEmptyListIfNoOrders()
    {
        var orders = CreateOrders();
        using var context = TestDbContextFactory.Create();
        
        var service = new OrderService(context);
        var result = await service.GetAllAsync(1, 10);
        
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task UpdateOrderAsync_ReturnsOrderDtoStoreToDb_WhenOrderIsUpdated()
    {
        var orders = CreateOrders();
        using var context = TestDbContextFactory.Create();
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();

        var orderUpdateDto = new OrderUpdateDto()
        {
            Status = "completed",
        };
        var orderId = 1;
        var service = new OrderService(context);

        var result = await service.UpdateOrderAsync(orderUpdateDto, orderId);
        await context.SaveChangesAsync();
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

        Assert.NotNull(order);
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Equal(result.Id, order.Id);
        Assert.Equal(orderId, result.Id);
    }
    
    [Fact]
    public async Task UpdateOrderAsync_ThrowsArgumentException_WhenStatusIsInvalid()
    {
        var orders = CreateOrders();
        using var context = TestDbContextFactory.Create();
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();

        var orderUpdateDto = new OrderUpdateDto()
        {
            Status = "complete",
        };
        var orderId = 1;
        var service = new OrderService(context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateOrderAsync(orderUpdateDto, orderId));
    }
    
    [Fact]
    public async Task UpdateOrderAsync_ThrowsKeyNotFoundException_WhenOrderDoesntExists()
    {
        var orders = CreateOrders();
        using var context = TestDbContextFactory.Create();
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();

        var orderUpdateDto = new OrderUpdateDto()
        {
            Status = "completed",
        };
        
        var orderId = 3;
        var service = new OrderService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateOrderAsync(orderUpdateDto, orderId));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOrderDetailesDto()
    {
        var orders = CreateOrders();
        var product = CreateProduct();
        orders.ForEach((o) => o.Items = new List<OrderItem>() { CreateOrderItem(o, product, o.Id) });
        await using var context = TestDbContextFactory.Create();
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();
        
        var service = new OrderService(context);
        var result = await service.GetByIdAsync(1);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.NotNull(result.Items);
        Assert.NotEmpty(result.Items);
        Assert.Single(result.Items);
        Assert.IsType<OrderDetailsDto>(result);
    }
    
    [Fact]
    public async Task GetByIdAsync_ThrowsKeyNotFoundException_WhenOrderDoesntExists()
    {
        var orders = CreateOrders();
        await using var context = TestDbContextFactory.Create();
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();
        
        var service = new OrderService(context);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByIdAsync(3));
    }
}