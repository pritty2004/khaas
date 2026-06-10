using Microsoft.EntityFrameworkCore;
using ShopSpark.Domain.Entities;

namespace ShopSpark.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // User
        mb.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();
        mb.Entity<User>()
            .Property(u => u.Role).HasDefaultValue("Customer");

        // Product
        mb.Entity<Product>()
            .Property(p => p.Price).HasColumnType("decimal(18,2)");
        mb.Entity<Product>()
            .Property(p => p.OriginalPrice).HasColumnType("decimal(18,2)");

        // CartItem — composite unique so a user can't double-add
        mb.Entity<CartItem>()
            .HasIndex(c => new { c.UserId, c.ProductId }).IsUnique();
        mb.Entity<CartItem>()
            .HasOne(c => c.User).WithMany(u => u.CartItems).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        mb.Entity<CartItem>()
            .HasOne(c => c.Product).WithMany(p => p.CartItems).HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.Cascade);

        // WishlistItem
        mb.Entity<WishlistItem>()
            .HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();
        mb.Entity<WishlistItem>()
            .HasOne(w => w.User).WithMany(u => u.WishlistItems).HasForeignKey(w => w.UserId).OnDelete(DeleteBehavior.Cascade);
        mb.Entity<WishlistItem>()
            .HasOne(w => w.Product).WithMany(p => p.WishlistItems).HasForeignKey(w => w.ProductId).OnDelete(DeleteBehavior.Cascade);

        // Order
        mb.Entity<Order>()
            .Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
        mb.Entity<Order>()
            .Property(o => o.ShippingCost).HasColumnType("decimal(18,2)");
        mb.Entity<Order>()
            .Property(o => o.Tax).HasColumnType("decimal(18,2)");
        mb.Entity<Order>()
            .Property(o => o.Total).HasColumnType("decimal(18,2)");
        mb.Entity<Order>()
            .HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Restrict);

        // OrderItem — no cascade from Product (product might be deleted later)
        mb.Entity<OrderItem>()
            .Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        mb.Entity<OrderItem>()
            .HasOne(i => i.Order).WithMany(o => o.OrderItems).HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        mb.Entity<OrderItem>()
            .HasOne(i => i.Product).WithMany(p => p.OrderItems).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);

        // NewsletterSubscription
        mb.Entity<NewsletterSubscription>()
            .HasIndex(n => n.Email).IsUnique();

        // Seed products matching the frontend
        mb.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Rania Kundan Kara",         Price = 24500, OriginalPrice = 28000, ImageUrl = "/images/product-1.jpg", Tag = "Bestseller", Category = "Kundan",    StockQuantity = 10 },
            new Product { Id = 2, Name = "Aara Slim Stack (Set of 3)", Price = 9800,                         ImageUrl = "/images/product-2.jpg", Tag = "New",        Category = "Minimal",   StockQuantity = 25 },
            new Product { Id = 3, Name = "Zara Filigree Cuff",         Price = 18200,                        ImageUrl = "/images/product-3.jpg", Tag = "",           Category = "Antique",   StockQuantity = 8  },
            new Product { Id = 4, Name = "Noor Polki Pair",            Price = 32000, OriginalPrice = 36500, ImageUrl = "/images/product-4.jpg", Tag = "Limited",    Category = "Meenakari", StockQuantity = 5  }
        );

        mb.Entity<Collection>().HasData(
            new Collection { Id = 1, Name = "Kundan Collection", ImageUrl = "/images/collection-kundan.jpg",    StartingPrice = "From ₹18,500", SortOrder = 1 },
            new Collection { Id = 2, Name = "Minimal Gold",      ImageUrl = "/images/collection-minimal.jpg",   StartingPrice = "From ₹8,200",  SortOrder = 2 },
            new Collection { Id = 3, Name = "Antique Stack",     ImageUrl = "/images/collection-antique.jpg",   StartingPrice = "From ₹12,000", SortOrder = 3 },
            new Collection { Id = 4, Name = "Meenakari Story",   ImageUrl = "/images/collection-meenakari.jpg", StartingPrice = "From ₹22,000", SortOrder = 4 }
        );
    }
}
