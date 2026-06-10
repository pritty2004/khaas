namespace ShopSpark.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer"; // Customer | Admin
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<NewsletterSubscription> NewsletterSubscriptions { get; set; } = new List<NewsletterSubscription>();
}
