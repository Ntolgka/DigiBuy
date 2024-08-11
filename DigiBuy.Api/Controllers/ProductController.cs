using DigiBuy.Application.Dtos.ProductDTOs;
using DigiBuy.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiBuy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService productService;

    public ProductController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProduct = await productService.CreateProductAsync(productDto);
        
        return Created(createdProduct.Name, createdProduct);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await productService.GetProductByIdAsync(id);
        return product != null ? Ok(product) : NotFound("Product not found.");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("by-category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByCategory(Guid categoryId)
    {
        var products = await productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }
    
    [HttpGet("by-status/{status}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByStatus(bool status)
    {
        var products = await productService.GetProductsByStatusAsync(status);
        return Ok(products);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDTO productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await productService.UpdateProductAsync(id, productDto);
        return NoContent();
    }
    
    [HttpPut("{id}/stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProductStock(Guid id, [FromBody] int newStock)
    {
        if (newStock < 0)
        {
            return BadRequest("Stock cannot be negative.");
        }

        try
        {
            await productService.UpdateProductStockAsync(id, newStock);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        await productService.DeleteProductAsync(id);
        return NoContent();
    }
    
    [HttpGet("{id}/categories")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetProductCategories(Guid id)
    {
        try
        {
            var categories = await productService.GetProductCategoriesAsync(id);
            return Ok(categories);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Product not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("{productId}/categories/{categoryId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddCategoryToProduct(Guid productId, Guid categoryId)
    {
        await productService.AddCategoryToProductAsync(productId, categoryId);
        return NoContent();
    }

    [HttpDelete("{productId}/categories/{categoryId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveCategoryFromProduct(Guid productId, Guid categoryId)
    {
        await productService.RemoveCategoryFromProductAsync(productId, categoryId);
        return NoContent();
    }
}