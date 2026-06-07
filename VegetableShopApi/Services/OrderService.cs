using Microsoft.EntityFrameworkCore;
using VegetableShopApi.Data;
using VegetableShopApi.DTOs.OrderDto;
using VegetableShopApi.DTOs.OrderItemDto;
using VegetableShopApi.DTOs.ProductDto;
using VegetableShopApi.Enums;
using VegetableShopApi.Exceptions;
using VegetableShopApi.Models;
using VegetableShopApi.Services.Interfaces;

namespace VegetableShopApi.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _dbContext;
    
    public OrderService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<OrderDto> CreateOrderAsync(OrderCreateDto orderDto)
    {
       ValidateOrder(orderDto);
       
        
        var groupedItems = orderDto.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new OrderItemCreateDto
            {
                ProductId = g.Key,
                Quantity = g.Sum(i => i.Quantity)
            })
            .ToList();
        
        var ids = groupedItems.Select(i => i.ProductId).ToList();
        
        var products = await _dbContext.Products
            .Where(p => ids.Contains(p.Id)).ToListAsync();
        
        Order order = new Order()
        {
            User = await GetOrCreateUserAsync(orderDto), 
            DeliveryAddress = orderDto.DeliveryAddress,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var productsById = products.ToDictionary(p => p.Id);
        ValidateOrderItemsExist(groupedItems, productsById);
        ValidateQuantityToUnit(groupedItems, productsById);
        ValidateStockAvailability(groupedItems, productsById);
        List<OrderItem> orderItems = CreateOrderItems(groupedItems, productsById, order);
        UpdateStock(groupedItems, productsById);
        
        
        order.Items = orderItems;
        order.TotalPrice = orderItems.Sum(i => i.TotalPrice);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
        return new OrderDto(order);
    }

    private void ValidateQuantityToUnit(List<OrderItemCreateDto> groupedItems, Dictionary<int, Product> productsById)
    {
        foreach (var orderedItem in groupedItems)
        {
            if (productsById[orderedItem.ProductId].Unit == UnitType.Pcs)
            {
                if (orderedItem.Quantity % 1 != 0)
                {
                    throw new ArgumentException(
                        $"Quantity must be a whole number for product with id {orderedItem.ProductId} as it sells by pcs"
                    );
                }
            }
        }
    }

    private void ValidateOrder(OrderCreateDto orderDto)
    {
        if (orderDto.Items == null || !orderDto.Items.Any())
            throw new ArgumentException("Order must contain at least one item.");
        if (orderDto.Items.Any(i => i.Quantity <= 0))
            throw new ArgumentException("Quantity must be greater than zero.");
        if (String.IsNullOrWhiteSpace(orderDto.CustomerName))
            throw new ArgumentException("Customer name is required.");
        if (String.IsNullOrWhiteSpace(orderDto.CustomerLastname))
            throw new ArgumentException("Customer last name is required.");
        if (String.IsNullOrWhiteSpace(orderDto.CustomerEmail))
            throw new ArgumentException("Customer email is required.");
        if (String.IsNullOrWhiteSpace(orderDto.CustomerPhone))
            throw new ArgumentException("Customer phone is required.");
        if (String.IsNullOrWhiteSpace(orderDto.DeliveryAddress))
            throw new ArgumentException("Delivery address is required.");
    }

    private async Task<User> GetOrCreateUserAsync(OrderCreateDto orderDto)
    {
        var orderEmail = orderDto.CustomerEmail.Trim().ToLower();
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == orderEmail);

        if (user == null)
        {
            user = new User()
            {
                FirstName = orderDto.CustomerName.Trim(),
                LastName = orderDto.CustomerLastname.Trim(),
                Email = orderEmail,
                Phone = orderDto.CustomerPhone.Trim(),
                Role = UserRole.Customer
            };
            await _dbContext.Users.AddAsync(user);
        }
        
        return user;
    }
    
    private void ValidateStockAvailability(List<OrderItemCreateDto> groupedItems, Dictionary<int, Product> productsById)
    {
        foreach (var orderedItem in groupedItems)
        {
            if (orderedItem.Quantity > productsById[orderedItem.ProductId].StockQuantity)
            {
                throw new InsufficientStockException(
                    orderedItem.ProductId,
                    orderedItem.Quantity,
                    productsById[orderedItem.ProductId].StockQuantity);
            }
        }
    }

    private void ValidateOrderItemsExist(List<OrderItemCreateDto> groupedItems, Dictionary<int, Product> productsById)
    {
        foreach (var item in groupedItems)
        {
            if (!productsById.ContainsKey(item.ProductId))
            {
                throw new KeyNotFoundException($"Product with id {item.ProductId} does not exist.");
            }
        }
    }

    private List<OrderItem> CreateOrderItems(List<OrderItemCreateDto> items, Dictionary<int, Product> productsById, Order order)
    {
        List<OrderItem> orderItems = new List<OrderItem>();
        foreach (var item in items)
        {
           orderItems.Add(OrderItem.CreateOrderItem(productsById[item.ProductId], item.Quantity, order));
        }
        
        return orderItems;
    }
    
    private void UpdateStock(List<OrderItemCreateDto> groupedItems, Dictionary<int, Product> productsById)
    {
        foreach (var orderedItem in groupedItems)
        {
            productsById[orderedItem.ProductId].StockQuantity -= orderedItem.Quantity;
        }
    }

    public async Task<PagedResult<OrderDto>> GetAllAsync(int page, int pageSize)
    {
        var oreders = await _dbContext.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .Include(o => o.User)
            .ToListAsync();
        return new PagedResult<OrderDto>(oreders.Select(o => new OrderDto(o))
            .ToList(), page, pageSize, await _dbContext.Orders.CountAsync());
    }

    public async Task<OrderDto> UpdateOrderAsync(OrderUpdateDto orderDto, int id)
    {
        var status = ValidateOrderUpdateDto(orderDto);
        var order = await _dbContext.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync( o=> o.Id == id) 
                    ?? throw new KeyNotFoundException($"Order with id {id} does not exist.");
        order.Status = status;
        await _dbContext.SaveChangesAsync();
        return new OrderDto(order);
    }

    private OrderStatus ValidateOrderUpdateDto(OrderUpdateDto orderDto)
    {
        if (string.IsNullOrWhiteSpace(orderDto.Status.ToString())) throw new ArgumentException("Status is required.");

        if (!Enum.TryParse(orderDto.Status.ToString(), true, out OrderStatus status))
        {
            throw new ArgumentException("Invalid status.");
        }

        return status;
    }
}