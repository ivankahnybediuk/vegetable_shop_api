namespace VegetableShopApi.DTOs.OrderDto;

public class OrderDto
{
    public int Id { get; set; }
    public required UserDto.UserDto Customer { get; set; }
    public required string DeliveryAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Status { get; set; }
    public decimal Total { get; set; }
}