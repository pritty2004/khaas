using System.ComponentModel.DataAnnotations;

namespace ShopSpark.Application.DTOs;

// ── Order ─────────────────────────────────────────────────────────────────────

public class PlaceOrderDto
{
    // Shipping / contact info (mirrors Payment.tsx form fields)
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string State { get; set; } = string.Empty;

    [Required]
    public string ZipCode { get; set; } = string.Empty;

    // Payment info (we only store last 4, never raw card data)
    [Required]
    public string CardNumber { get; set; } = string.Empty; // full number — strip all but last 4 server side

    [Required]
    public string ExpiryDate { get; set; } = string.Empty; // MM/YY

    [Required]
    public string Cvv { get; set; } = string.Empty; // used for payment gateway, never persisted
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public class OrderDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string CardLastFour { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

// ── Newsletter ───────────────────────────────────────────────────────────────

public class NewsletterSubscribeDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}

// ── Dashboard ────────────────────────────────────────────────────────────────

public class DashboardSummaryDto
{
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
    public List<TopProductDto> TopProducts { get; set; } = new();
    public List<MonthlySalesDto> MonthlySales { get; set; } = new();
}

public class RecentOrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalSold { get; set; }
    public decimal Revenue { get; set; }
}

public class MonthlySalesDto
{
    public string Month { get; set; } = string.Empty; // e.g. "Jan 2026"
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}

// ── Generic ───────────────────────────────────────────────────────────────────

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

public class MessageResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
