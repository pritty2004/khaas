using Microsoft.EntityFrameworkCore;
using ShopSpark.Application.Interfaces;
using ShopSpark.Domain.Entities;
using ShopSpark.Infrastructure.Data;
using System.Linq.Expressions;

namespace ShopSpark.Infrastructure.Repositories;

// ── Generic ──────────────────────────────────────────────────────────────────

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _set;

    public Repository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _set.ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _set.Where(predicate).ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await _set.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        => predicate == null
            ? await _set.CountAsync()
            : await _set.CountAsync(predicate);
}

// ── User ─────────────────────────────────────────────────────────────────────

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public async Task<User?> GetByEmailAsync(string email)
        => await _set.FirstOrDefaultAsync(u => u.Email == email);
}

// ── Product ───────────────────────────────────────────────────────────────────

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext db) : base(db) { }

    public async Task<(List<Product> Items, int Total)> GetPagedAsync(
        string? category, string? tag, string? search, int page, int pageSize)
    {
        var query = _set.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category.ToLower() == category.ToLower());

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(p => p.Tag.ToLower() == tag.ToLower());

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}

// ── Cart ─────────────────────────────────────────────────────────────────────

public class CartRepository : Repository<CartItem>, ICartRepository
{
    public CartRepository(AppDbContext db) : base(db) { }

    public async Task<List<CartItem>> GetCartWithProductsAsync(int userId)
        => await _set.Include(c => c.Product)
                     .Where(c => c.UserId == userId)
                     .ToListAsync();

    public async Task<CartItem?> GetUserCartItemAsync(int userId, int productId)
        => await _set.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
}

// ── Order ─────────────────────────────────────────────────────────────────────

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext db) : base(db) { }

    public async Task<Order?> GetOrderWithItemsAsync(int orderId)
        => await _set.Include(o => o.OrderItems)
                     .FirstOrDefaultAsync(o => o.Id == orderId);

    public async Task<List<Order>> GetUserOrdersAsync(int userId)
        => await _set.Include(o => o.OrderItems)
                     .Where(o => o.UserId == userId)
                     .OrderByDescending(o => o.CreatedAt)
                     .ToListAsync();

    public async Task<List<Order>> GetAllPagedAsync(int page, int pageSize)
        => await _set.Include(o => o.OrderItems)
                     .OrderByDescending(o => o.CreatedAt)
                     .Skip((page - 1) * pageSize)
                     .Take(pageSize)
                     .ToListAsync();

    public async Task<List<Order>> GetRecentOrdersAsync(int count)
        => await _set.OrderByDescending(o => o.CreatedAt)
                     .Take(count)
                     .ToListAsync();

    public async Task<List<(int ProductId, string Name, int TotalSold, decimal Revenue)>> GetTopProductsAsync(int count)
    {
        var result = await _db.OrderItems
            .GroupBy(i => new { i.ProductId, i.ProductName })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ProductName,
                TotalSold = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.UnitPrice * i.Quantity)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(count)
            .ToListAsync();

        return result.Select(r => (r.ProductId, r.ProductName, r.TotalSold, r.Revenue)).ToList();
    }

    public async Task<List<(string Month, decimal Revenue, int Count)>> GetMonthlySalesAsync(int months)
    {
        var since = DateTime.UtcNow.AddMonths(-months);
        var result = await _set
            .Where(o => o.CreatedAt >= since && o.PaymentStatus == "Paid")
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Revenue = g.Sum(o => o.Total),
                Count = g.Count()
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        return result.Select(r =>
            ($"{new DateTime(r.Year, r.Month, 1):MMM yyyy}", r.Revenue, r.Count)).ToList();
    }
}

// ── Wishlist ──────────────────────────────────────────────────────────────────

public class WishlistRepository : Repository<WishlistItem>, IWishlistRepository
{
    public WishlistRepository(AppDbContext db) : base(db) { }

    public async Task<List<WishlistItem>> GetWishlistWithProductsAsync(int userId)
        => await _set.Include(w => w.Product)
                     .Where(w => w.UserId == userId)
                     .ToListAsync();

    public async Task<WishlistItem?> GetUserWishlistItemAsync(int userId, int productId)
        => await _set.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
}

// ── Newsletter ────────────────────────────────────────────────────────────────

public class NewsletterRepository : Repository<NewsletterSubscription>, INewsletterRepository
{
    public NewsletterRepository(AppDbContext db) : base(db) { }

    public async Task<NewsletterSubscription?> GetByEmailAsync(string email)
        => await _set.FirstOrDefaultAsync(n => n.Email == email);
}
