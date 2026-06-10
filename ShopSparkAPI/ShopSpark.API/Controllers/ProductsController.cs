using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;

namespace ShopSpark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;

    public ProductsController(IProductService products) => _products = products;

    /// <summary>
    /// Get paginated products. Optional filters: category, tag, search.
    /// Matches frontend ProductShowcase and Collections browsing.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? category,
        [FromQuery] string? tag,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var result = await _products.GetProductsAsync(category, tag, search, page, pageSize);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Get a single product by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _products.GetProductByIdAsync(id);
        if (product == null) return NotFound(new { success = false, message = "Product not found." });
        return Ok(new { success = true, data = product });
    }

    /// <summary>Create a new product (Admin only).</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = await _products.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id },
            new { success = true, message = "Product created.", data = product });
    }

    /// <summary>Update a product (Admin only).</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var product = await _products.UpdateProductAsync(id, dto);
        if (product == null) return NotFound(new { success = false, message = "Product not found." });
        return Ok(new { success = true, message = "Product updated.", data = product });
    }

    /// <summary>Delete a product (Admin only).</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _products.DeleteProductAsync(id);
        if (!deleted) return NotFound(new { success = false, message = "Product not found." });
        return Ok(new { success = true, message = "Product deleted." });
    }
}
