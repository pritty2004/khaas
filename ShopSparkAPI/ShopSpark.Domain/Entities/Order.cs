namespace ShopSpark.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Shipping / contact
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    // Totals
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    // Status: Pending | Processing | Shipped | Delivered | Cancelled
    public string Status { get; set; } = "Pending";

    // Payment: Paid | Failed | Pending
    public string PaymentStatus { get; set; } = "Pending";

    // Masked card last 4 digits for display
    public string CardLastFour { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
