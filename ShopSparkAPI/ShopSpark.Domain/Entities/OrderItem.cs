namespace ShopSpark.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty; // snapshot at time of order
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;

    // Navigation
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
