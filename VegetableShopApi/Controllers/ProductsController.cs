using Microsoft.AspNetCore.Mvc;
using VegetableShopApi.Services.Interfaces;

namespace VegetableShopApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
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
            return StatusCode(500, ex.Message);
        }
    }
    
}