namespace VegetableShopApi.DTOs.OrderItemDto;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public required string Unit { get; set; }
    public decimal TotalPrice { get; set; }
}