using VegetableShopApi.Models;

namespace VegetableShopApi.DTOs.OrderItemDto;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Unit { get; set; }
    public decimal TotalPrice { get; set; }

    public OrderItemDto(OrderItem orderItem)
    {
        ProductId = orderItem.ProductId;
        ProductName = orderItem.Product.Name;
        Quantity = orderItem.Quantity;
        UnitPrice = orderItem.Product.Price;
        Unit = orderItem.Product.Unit.ToString().ToLower();
        TotalPrice = orderItem.Quantity * orderItem.Product.Price;
    }
}