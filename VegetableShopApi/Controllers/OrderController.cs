using Microsoft.AspNetCore.Mvc;
using VegetableShopApi.DTOs.OrderDto;
using VegetableShopApi.Exceptions;
using VegetableShopApi.Services.Interfaces;

namespace VegetableShopApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private IOrderService _orderService;
    private ILogger<OrderController>? _logger;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
    {
        try
        {
            if (orderDto == null) return BadRequest("Order is required");
            return Ok(await _orderService.CreateOrderAsync(orderDto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error creating order");
            return StatusCode(500, ex.Message);
        }
    }
    
}