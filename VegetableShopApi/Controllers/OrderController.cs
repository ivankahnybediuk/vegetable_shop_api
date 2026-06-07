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

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 9)
    {
        try
        {
            if (page < 1 || pageSize < 1) return BadRequest("Page and PageSize must be greater than 0");
            return Ok(await _orderService.GetAllAsync(page, pageSize));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting orders");
            return StatusCode(500, ex.Message);
        }
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

    [HttpPatch]
    [Route("{id:int}")]
    public async Task<IActionResult> UpdateOrder([FromBody] OrderUpdateDto orderDto, [FromRoute] int id)
    {
        try
        {
            if (orderDto == null) return BadRequest("Order is required");
            if (id <= 0) return BadRequest("Id must be greater than 0");
            return Ok(await _orderService.UpdateOrderAsync(orderDto, id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating order");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            if (id <= 0) return BadRequest("Id must be greater than 0");
            return Ok(await _orderService.GetByIdAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting order by id");
            return StatusCode(500, ex.Message);
        }
    }
    
}