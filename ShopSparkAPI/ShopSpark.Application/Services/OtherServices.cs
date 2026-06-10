using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;
using ShopSpark.Domain.Entities;

namespace ShopSpark.Application.Services;

// ── Wishlist ─────────────────────────────────────────────────────────────────

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _uow;
    public WishlistService(IUnitOfWork uow) => _uow = uow;

    public async Task<List<WishlistItemDto>> GetWishlistAsync(int userId)
    {
        var items = await _uow.Wishlists.GetWishlistWithProductsAsync(userId);
        return items.Select(i => new WishlistItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            ProductPrice = i.Product.Price,
            ProductOriginalPrice = i.Product.OriginalPrice,
            ProductImageUrl = i.Product.ImageUrl,
            ProductTag = i.Product.Tag,
            AddedAt = i.AddedAt
        }).ToList();
    }

    public async Task<WishlistItemDto> AddItemAsync(int userId, AddToWishlistDto dto)
    {
        var product = await _uow.Products.GetByIdAsync(dto.ProductId)
            ?? throw new KeyNotFoundException("Product not found.");

        var existing = await _uow.Wishlists.GetUserWishlistItemAsync(userId, dto.ProductId);
        if (existing != null)
            return new WishlistItemDto { Id = existing.Id, ProductId = existing.ProductId };

        var item = new WishlistItem { UserId = userId, ProductId = dto.ProductId };
        await _uow.Wishlists.AddAsync(item);
        await _uow.SaveChangesAsync();

        return new WishlistItemDto
        {
            Id = item.Id,
            ProductId = product.Id,
            ProductName = product.Name,
            ProductPrice = product.Price,
            ProductOriginalPrice = product.OriginalPrice,
            ProductImageUrl = product.ImageUrl,
            ProductTag = product.Tag,
            AddedAt = item.AddedAt
        };
    }

    public async Task<bool> RemoveItemAsync(int userId, int productId)
    {
        var item = await _uow.Wishlists.GetUserWishlistItemAsync(userId, productId);
        if (item == null) return false;
        await _uow.Wishlists.DeleteAsync(item);
        await _uow.SaveChangesAsync();
        return true;
    }
}

// ── Order ─────────────────────────────────────────────────────────────────────

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly ICartService _cart;
    public OrderService(IUnitOfWork uow, ICartService cart) { _uow = uow; _cart = cart; }

    public async Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderDto dto)
    {
        // 1. Get cart
        var cartData = await _cart.GetCartAsync(userId);
        if (!cartData.Items.Any())
            throw new InvalidOperationException("Cannot place an order with an empty cart.");

        // 2. Deduct stock
        foreach (var ci in cartData.Items)
        {
            var product = await _uow.Products.GetByIdAsync(ci.ProductId)!;
            if (product!.StockQuantity < ci.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}.");
            product.StockQuantity -= ci.Quantity;
            await _uow.Products.UpdateAsync(product);
        }

        // 3. Create order
        var order = new Order
        {
            UserId = userId,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            Subtotal = cartData.Subtotal,
            ShippingCost = cartData.ShippingCost,
            Tax = cartData.Tax,
            Total = cartData.Total,
            Status = "Processing",
            PaymentStatus = "Paid",
            // Never store raw card numbers
            CardLastFour = dto.CardNumber.Replace(" ", "").Length >= 4
                ? dto.CardNumber.Replace(" ", "")[^4..]
                : "****",
            OrderItems = cartData.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.ProductPrice,
                Quantity = i.Quantity
            }).ToList()
        };

        await _uow.Orders.AddAsync(order);
        await _uow.SaveChangesAsync();

        // 4. Clear cart
        await _cart.ClearCartAsync(userId);

        return MapOrderDto(order);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int userId)
    {
        var order = await _uow.Orders.GetOrderWithItemsAsync(orderId);
        if (order == null || order.UserId != userId) return null;
        return MapOrderDto(order);
    }

    public async Task<List<OrderDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _uow.Orders.GetUserOrdersAsync(userId);
        return orders.Select(MapOrderDto).ToList();
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync(int page, int pageSize)
    {
        var orders = await _uow.Orders.GetAllPagedAsync(page, pageSize);
        return orders.Select(MapOrderDto).ToList();
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _uow.Orders.GetOrderWithItemsAsync(orderId);
        if (order == null) return null;
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();
        return MapOrderDto(order);
    }

    private static OrderDto MapOrderDto(Order o) => new()
    {
        Id = o.Id,
        FullName = o.FullName,
        Email = o.Email,
        Phone = o.Phone,
        Address = o.Address,
        City = o.City,
        State = o.State,
        ZipCode = o.ZipCode,
        Subtotal = o.Subtotal,
        ShippingCost = o.ShippingCost,
        Tax = o.Tax,
        Total = o.Total,
        Status = o.Status,
        PaymentStatus = o.PaymentStatus,
        CardLastFour = o.CardLastFour,
        CreatedAt = o.CreatedAt,
        Items = o.OrderItems?.Select(i => new OrderItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity,
            LineTotal = i.UnitPrice * i.Quantity
        }).ToList() ?? new()
    };
}

// ── Newsletter ───────────────────────────────────────────────────────────────

public class NewsletterService : INewsletterService
{
    private readonly IUnitOfWork _uow;
    public NewsletterService(IUnitOfWork uow) => _uow = uow;

    public async Task<bool> SubscribeAsync(NewsletterSubscribeDto dto, int? userId)
    {
        var existing = await _uow.Newsletters.GetByEmailAsync(dto.Email.ToLowerInvariant());
        if (existing != null)
        {
            existing.IsActive = true;
            await _uow.Newsletters.UpdateAsync(existing);
        }
        else
        {
            await _uow.Newsletters.AddAsync(new NewsletterSubscription
            {
                Email = dto.Email.ToLowerInvariant(),
                UserId = userId,
                IsActive = true
            });
        }
        await _uow.SaveChangesAsync();
        return true;
    }
}

// ── Dashboard ─────────────────────────────────────────────────────────────────

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    public DashboardService(IUnitOfWork uow) => _uow = uow;

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var totalOrders = await _uow.Orders.CountAsync();
        var totalCustomers = await _uow.Users.CountAsync(u => u.Role == "Customer");
        var totalProducts = await _uow.Products.CountAsync();
        var lowStock = await _uow.Products.CountAsync(p => p.StockQuantity < 5 && p.IsActive);

        var allOrders = await _uow.Orders.GetAllAsync();
        var totalRevenue = allOrders
            .Where(o => o.PaymentStatus == "Paid")
            .Sum(o => o.Total);

        var recentOrders = await _uow.Orders.GetRecentOrdersAsync(5);
        var topProducts = await _uow.Orders.GetTopProductsAsync(5);
        var monthlySales = await _uow.Orders.GetMonthlySalesAsync(6);

        return new DashboardSummaryDto
        {
            TotalOrders = totalOrders,
            TotalCustomers = totalCustomers,
            TotalRevenue = totalRevenue,
            TotalProducts = totalProducts,
            LowStockProducts = lowStock,
            RecentOrders = recentOrders.Select(o => new RecentOrderDto
            {
                Id = o.Id,
                CustomerName = o.FullName,
                Total = o.Total,
                Status = o.Status,
                CreatedAt = o.CreatedAt
            }).ToList(),
            TopProducts = topProducts.Select(t => new TopProductDto
            {
                ProductId = t.ProductId,
                ProductName = t.Name,
                TotalSold = t.TotalSold,
                Revenue = t.Revenue
            }).ToList(),
            MonthlySales = monthlySales.Select(m => new MonthlySalesDto
            {
                Month = m.Month,
                Revenue = m.Revenue,
                OrderCount = m.Count
            }).ToList()
        };
    }
}
