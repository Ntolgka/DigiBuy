using System.Security.Claims;
using DigiBuy.Application.Dtos.UserDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiBuy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;

    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDTO userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await userService.RegisterAsync(userDto);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }
    
    [HttpPost("register-admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RegisterAdminAsync(CreateUserDTO userDto)
    {
        try
        {
            var user = await userService.RegisterAdminAsync(userDto);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var token = await userService.LoginAsync(loginDto);
        return Ok(new { Token = token });
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await userService.LogoutAsync();
        return NoContent();
    }
    
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await userService.ChangePasswordAsync(changePasswordDto);
        return NoContent();
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return user != null ? Ok(user) : NotFound("User not found.");
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != userDto.Id && !User.IsInRole("Admin"))
        {
            return Forbid(); // User does not have permission to update this account
        }

        await userService.UpdateUserAsync(userDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != id && !User.IsInRole(UserRole.Admin.ToString()))
        {
            return Forbid(); // User does not have permission to delete this account
        }

        await userService.DeleteUserAsync(id);
        return NoContent();
    }
}