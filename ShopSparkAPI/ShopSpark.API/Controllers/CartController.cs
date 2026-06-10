using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;
using System.Security.Claims;

namespace ShopSpark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cart;
    public CartController(ICartService cart) => _cart = cart;

    private int UserId => int.Parse(User.FindFirstValue("userId")!);

    /// <summary>Get the current user's cart (matches Cart.tsx order summary).</summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cart = await _cart.GetCartAsync(UserId);
        return Ok(new { success = true, data = cart });
    }

    /// <summary>Add a product to cart (called from ProductShowcase Add-to-Cart button).</summary>
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
    {
        var cart = await _cart.AddItemAsync(UserId, dto);
        return Ok(new { success = true, message = "Item added to cart.", data = cart });
    }

    /// <summary>Update quantity of a cart item.</summary>
    [HttpPut("items/{cartItemId:int}")]
    public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] UpdateCartItemDto dto)
    {
        var cart = await _cart.UpdateItemAsync(UserId, cartItemId, dto);
        return Ok(new { success = true, data = cart });
    }

    /// <summary>Remove a specific item from cart.</summary>
    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        var cart = await _cart.RemoveItemAsync(UserId, cartItemId);
        return Ok(new { success = true, message = "Item removed.", data = cart });
    }

    /// <summary>Clear the entire cart.</summary>
    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        await _cart.ClearCartAsync(UserId);
        return Ok(new { success = true, message = "Cart cleared." });
    }
}
