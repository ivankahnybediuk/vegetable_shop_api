using VegetableShopApi.Enums;

namespace VegetableShopApi.DTOs.ProductDto;

public abstract class ProductBaseDto
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public required string Unit { get; set; }
    public decimal StockQuantity { get; set; }
    public required string ImageUrl { get; set; }
    public int CategoryId { get; set; }
}