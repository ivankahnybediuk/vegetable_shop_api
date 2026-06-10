using Microsoft.AspNetCore.Mvc;
using VegetableShopApi.Services.Interfaces;

namespace VegetableShopApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private IProductService _productService;
    private ILogger<ProductsController>? _logger;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 9)
    {
        try
        {
            if (page < 1 || pageSize < 1) return BadRequest("Page and PageSize must be greater than 0");
            var products = await _productService.GetAllPaginatedAsync(page, pageSize);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting products");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetProductsByCategoryId(
        [FromRoute] int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 9)
    {
        try
        {
            if (page < 1 || pageSize < 1) return BadRequest("Page and PageSize must be greater than 0");
            var products = await _productService.GetByCategoryIdAsync(categoryId, page, pageSize);
            return Ok(products);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting products by category");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetProductsByName(
        [FromQuery] string name,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 9)
    {
        try
        {
            if (page < 1 || pageSize < 1) return BadRequest("Page and PageSize must be greater than 0");
            if (string.IsNullOrEmpty(name)) return BadRequest("Name is required");
            if (name.Length < 3) return BadRequest("Name must be at least 3 characters long");
            
            var products = await _productService.GetByNameAsync(name, page, pageSize);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting products by name");
            return StatusCode(500, ex.Message);
        }
    }
    
}