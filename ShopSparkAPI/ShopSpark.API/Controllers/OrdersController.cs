using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;
using System.Security.Claims;

namespace ShopSpark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    public OrdersController(IOrderService orders) => _orders = orders;

    private int UserId => int.Parse(User.FindFirstValue("userId")!);

    /// <summary>
    /// Place a new order from the current cart.
    /// Matches the Payment.tsx form: fullName, email, phone, address, city, state, zipCode, cardNumber, expiryDate, cvv.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        var order = await _orders.PlaceOrderAsync(UserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = order.Id },
            new { success = true, message = "Order placed successfully!", data = order });
    }

    /// <summary>Get all orders for the logged-in user.</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var orders = await _orders.GetUserOrdersAsync(UserId);
        return Ok(new { success = true, data = orders });
    }

    /// <summary>Get a single order by ID (only if it belongs to the user).</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orders.GetOrderByIdAsync(id, UserId);
        if (order == null) return NotFound(new { success = false, message = "Order not found." });
        return Ok(new { success = true, data = order });
    }

    // ── Admin ────────────────────────────────────────────────────────────────

    /// <summary>Get all orders — Admin only.</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin/all")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var orders = await _orders.GetAllOrdersAsync(page, pageSize);
        return Ok(new { success = true, data = orders });
    }

    /// <summary>Update order status — Admin only.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        var order = await _orders.UpdateOrderStatusAsync(id, dto.Status);
        if (order == null) return NotFound(new { success = false, message = "Order not found." });
        return Ok(new { success = true, message = "Status updated.", data = order });
    }
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}
