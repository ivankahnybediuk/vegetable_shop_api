namespace DefaultNamespace;
using VegetableShopApi.Models;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }

    public CategoryDto(Category category)
    {
        Id = category.Id;
        Name = category.Name;
        ImageUrl = category.IconUrl;
    }
}