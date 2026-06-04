using Microsoft.AspNetCore.Mvc;
using VegetableShopApi.Services.Interfaces;

namespace VegetableShopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 9)
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
    
}