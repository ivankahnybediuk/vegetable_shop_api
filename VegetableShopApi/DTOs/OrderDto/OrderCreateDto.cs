using System.Text.Json.Serialization;
using VegetableShopApi.DTOs.OrderItemDto;

namespace VegetableShopApi.DTOs.OrderDto;

public class OrderCreateDto
{
    [JsonPropertyName("name")]
    public string? CustomerName { get; set; }
    
    [JsonPropertyName("lastname")]
    public string? CustomerLastname { get; set; }
    
    [JsonPropertyName("email")]
    public string? CustomerEmail { get; set; }
    
    [JsonPropertyName("phone")]
    public string? CustomerPhone { get; set; }
    
    [JsonPropertyName("address")]
    public string? DeliveryAddress { get; set; }
    
    public List<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();
}