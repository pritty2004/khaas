using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;

namespace ShopSpark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly ICollectionService _collections;
    public CollectionsController(ICollectionService collections) => _collections = collections;

    /// <summary>Get all featured collections (matches CollectionsSection.tsx).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _collections.GetCollectionsAsync();
        return Ok(new { success = true, data = result });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCollectionDto dto)
    {
        var col = await _collections.CreateCollectionAsync(dto);
        return Ok(new { success = true, data = col });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _collections.DeleteCollectionAsync(id);
        return Ok(new { success = true, message = "Collection deleted." });
    }
}
