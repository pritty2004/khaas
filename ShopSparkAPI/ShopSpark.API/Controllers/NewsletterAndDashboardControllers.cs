using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;
using System.Security.Claims;

namespace ShopSpark.API.Controllers;

// ── Newsletter ───────────────────────────────────────────────────────────────

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly INewsletterService _newsletter;
    public NewsletterController(INewsletterService newsletter) => _newsletter = newsletter;

    /// <summary>
    /// Subscribe to the newsletter — matches the Footer "Join the Inner Circle" form.
    /// Works for both guests and logged-in users.
    /// </summary>
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] NewsletterSubscribeDto dto)
    {
        int? userId = null;
        var userIdClaim = User.FindFirstValue("userId");
        if (!string.IsNullOrEmpty(userIdClaim))
            userId = int.Parse(userIdClaim);

        await _newsletter.SubscribeAsync(dto, userId);
        return Ok(new { success = true, message = "Thank you for subscribing!" });
    }
}

// ── Dashboard ────────────────────────────────────────────────────────────────

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboard;
    public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    /// <summary>
    /// Admin dashboard summary: revenue, orders, customers, top products, monthly sales.
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _dashboard.GetSummaryAsync();
        return Ok(new { success = true, data = summary });
    }
}
