using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;
using Shared.Models;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly OrderManagementService _orderService;

    public OrderController(OrderManagementService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Create a new order.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        try
        {
            var result = await _orderService.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id },
                ApiResponse<Order>.Ok(result, "Order created successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Failed to create order", ex));
        }
    }

    /// <summary>
    /// Get orders with an optional JSON filter.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] string? filter)
    {
        try
        {
            var ordersJson = await _orderService.GetOrdersAsync(filter);
            var parsedNode = System.Text.Json.Nodes.JsonNode.Parse(ordersJson);
            return Ok(ApiResponse<System.Text.Json.Nodes.JsonNode>.Ok(parsedNode));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Failed to get orders", ex));
        }
    }

    /// <summary>
    /// Get a specific order by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(string id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse.Fail("Order not found"));

            return Ok(ApiResponse<Order>.Ok(order));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Failed to get order", ex));
        }
    }
}
