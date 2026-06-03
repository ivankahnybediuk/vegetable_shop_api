namespace VegetableShopApi.DTOs.OrderItemDto;

public class OrderItemCreateDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
}