using System.Security.Claims;
using DigiBuy.Application.Dtos.CheckoutDTOs;
using DigiBuy.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiBuy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        this.checkoutService = checkoutService;
    }

    [HttpPost("process")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ProcessCheckout([FromQuery] string orderId, [FromQuery] string couponCode, [FromQuery] bool usePoints)
    {
        try
        {
            var result = await checkoutService.CheckoutAsync(orderId, couponCode, User, usePoints);
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

