using DigiBuy.Application.Dtos.OrderDTOs;
using DigiBuy.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigiBuy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost]
    [Route("process")]
    public async Task<IActionResult> ProcessCheckout([FromBody] CreateOrderDTO orderDto, [FromQuery] string userId)
    {
        try
        {
            var result = await _checkoutService.CheckoutAsync(orderDto, userId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while processing the checkout." });
        }
    }
}