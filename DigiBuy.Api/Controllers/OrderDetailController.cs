using System.Security.Claims;
using DigiBuy.Application.Dtos.OrderDetailDTOs;
using DigiBuy.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiBuy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderDetailController : ControllerBase
{
    private readonly IOrderDetailService orderDetailService;

    public OrderDetailController(IOrderDetailService orderDetailService)
    {
        this.orderDetailService = orderDetailService;
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateOrderDetail([FromBody] CreateOrderDetailDTO orderDetailDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User ID not found in claims." });
            }

            var createdOrderDetail = await orderDetailService.CreateOrderDetailAsync(orderDetailDto, userId);
            return Created(nameof(GetOrderDetailById), createdOrderDetail);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetOrderDetailById(Guid id)
    {
        try
        {
            var orderDetail = await orderDetailService.GetOrderDetailByIdAsync(id);
            return Ok(orderDetail);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Order detail not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrderDetails()
    {
        var orderDetails = await orderDetailService.GetAllOrderDetailsAsync();
        return Ok(orderDetails);
    }

    [HttpGet("order/{orderId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetOrderDetailsByOrderId(Guid orderId)
    {
        try
        {
            var orderDetails = await orderDetailService.GetOrderDetailsByOrderIdAsync(orderId);
            return Ok(orderDetails);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateOrderDetail(Guid id, [FromBody] UpdateOrderDetailDTO orderDetailDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await orderDetailService.UpdateOrderDetailAsync(id, orderDetailDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Order detail not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrderDetail(Guid id)
    {
        try
        {
            await orderDetailService.DeleteOrderDetailAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Order detail not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}