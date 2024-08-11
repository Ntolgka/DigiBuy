using System.Security.Claims;
using DigiBuy.Application.Dtos.OrderDTOs;
using DigiBuy.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiBuy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService orderService;

    public OrderController(IOrderService orderService)
    {
        this.orderService = orderService;
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO orderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var createdOrder = await orderService.CreateOrderAsync(orderDto, userId);
            return Created(nameof(GetOrderById), createdOrder);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        try
        {
            var order = await orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Order not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    [HttpGet("by-order-number/{orderNumber}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetOrderByOrderNumber(string orderNumber)
    {
        try
        {
            var order = await orderService.GetOrderByOrderNumberAsync(orderNumber);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Order not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetOrdersByUserId(string userId)
    {
        try
        {
            var orders = await orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    [HttpGet("active/user")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetActiveOrdersByUserId()
    {
        try
        {
            var orders = await orderService.GetActiveOrdersByUserIdAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("inactive/user")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetInactiveOrdersByUserId()
    {
        try
        {
            var orders = await orderService.GetInactiveOrdersByUserIdAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderDTO orderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await orderService.UpdateOrderAsync(id, orderDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Order not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        try
        {
            await orderService.DeleteOrderAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Order not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}