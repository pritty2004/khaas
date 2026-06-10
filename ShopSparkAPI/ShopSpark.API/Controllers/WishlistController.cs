using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;
using System.Security.Claims;

namespace ShopSpark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlist;
    public WishlistController(IWishlistService wishlist) => _wishlist = wishlist;

    private int UserId => int.Parse(User.FindFirstValue("userId")!);

    /// <summary>Get user's wishlist (triggered by Heart button in ProductShowcase).</summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var items = await _wishlist.GetWishlistAsync(UserId);
        return Ok(new { success = true, data = items });
    }

    /// <summary>Add a product to wishlist.</summary>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddToWishlistDto dto)
    {
        var item = await _wishlist.AddItemAsync(UserId, dto);
        return Ok(new { success = true, message = "Added to wishlist.", data = item });
    }

    /// <summary>Remove a product from wishlist by productId.</summary>
    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Remove(int productId)
    {
        var removed = await _wishlist.RemoveItemAsync(UserId, productId);
        if (!removed) return NotFound(new { success = false, message = "Item not found in wishlist." });
        return Ok(new { success = true, message = "Removed from wishlist." });
    }
}
