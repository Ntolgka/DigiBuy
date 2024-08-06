using DigiBuy.Application.Dtos.CouponDTOs;
using DigiBuy.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiBuy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponController : ControllerBase
{
    private readonly ICouponService couponService;

    public CouponController(ICouponService couponService)
    {
        this.couponService = couponService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDTO couponDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdCoupon = await couponService.CreateCouponAsync(couponDto);
            return Created(nameof(GetCouponById), createdCoupon);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCouponById(Guid id)
    {
        var coupon = await couponService.GetCouponByIdAsync(id);
        return coupon != null ? Ok(coupon) : NotFound("Coupon not found.");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCoupons()
    {
        var coupons = await couponService.GetAllCouponsAsync();
        return Ok(coupons);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCoupon(Guid id, [FromBody] UpdateCouponDTO couponDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await couponService.UpdateCouponAsync(id, couponDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Coupon not found.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCoupon(Guid id)
    {
        try
        {
            await couponService.DeleteCouponAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Coupon not found.");
        }
    }
}