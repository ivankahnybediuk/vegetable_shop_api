namespace VegetableShopApi.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? IconUrl { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();

}