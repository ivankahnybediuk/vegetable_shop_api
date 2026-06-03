using VegetableShopApi.Enums;

namespace VegetableShopApi.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public required string DeliveryAddress { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } 
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
}