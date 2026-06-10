namespace ShopSpark.Domain.Entities;

public class NewsletterSubscription
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? User { get; set; }
}
