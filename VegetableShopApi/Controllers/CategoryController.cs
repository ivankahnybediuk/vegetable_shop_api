using DefaultNamespace;
using Microsoft.AspNetCore.Mvc;

namespace VegetableShopApi.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private ICategoryService _categoryService;
    private ILogger<CategoryController>? _logger;
    
    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting categories");
            return StatusCode(500, ex.Message);
        }
    }
}