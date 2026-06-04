using VegetableShopApi.Enums;
using VegetableShopApi.Models;

namespace VegetableShopApi.DTOs.ProductDto;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Unit { get; set; }
    public decimal StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    public ProductDto(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Price = product.Price;
        Unit = product.Unit.ToString().ToLower();
        StockQuantity = product.StockQuantity;
        ImageUrl = product.ImageUrl;
        CategoryId = product.CategoryId;
        CategoryName = product.Category.Name;
    }
}