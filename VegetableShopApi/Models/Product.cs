using VegetableShopApi.Enums;

namespace VegetableShopApi.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public UnitType Unit { get; set; }
    public decimal StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
}